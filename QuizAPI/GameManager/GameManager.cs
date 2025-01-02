using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.Commands;
using QuizAPI.Dto;
using QuizAPI.HelperMethods;
using QuizAPI.Models;
using QuizAPI.Repository;
using QuizAPI.Services;
using QuizAPI.UserAccessor;
using System;
using System.Data.SQLite;
using System.Net.Http;
using System.Text.Json;

namespace QuizAPI.GameManager
{
    public class GameManager : IGameManager
    {
        private readonly string _connectionString;
        private readonly int allowedActiveGameSessions;
        private readonly IConfiguration _configuration;
        private readonly IActiveGameSessionsCommands _activeGameSessionsCommands;
        private readonly IActiveGameSessionsRepository _activeGameSessionsRepository;
        private readonly ITempGameSessionCommands _tempGameSessionCommands;
        private readonly ITempGameSessionRepository _tempGameSessionRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ICashedGameSessions _cashedGameSessions;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserAccessor _userAccessor;

        public GameManager(IConfiguration configuration, IActiveGameSessionsCommands activeGameSessionsCommands, IActiveGameSessionsRepository activeGameSessionsRepository,
                           ITempGameSessionCommands tempGameSessionCommands, ITempGameSessionRepository tempGameSessionRepository, ICashedGameSessions cashedGameSessions,
                           IQuestionRepository questionRepository, IAuthenticationService authenticationService, IUserAccessor userAccessor)
        {
            _configuration = configuration;
            _activeGameSessionsCommands = activeGameSessionsCommands;
            _activeGameSessionsRepository = activeGameSessionsRepository;
            _tempGameSessionCommands = tempGameSessionCommands;
            _tempGameSessionRepository = tempGameSessionRepository;
            _cashedGameSessions = cashedGameSessions;
            _questionRepository = questionRepository;
            _authenticationService = authenticationService;
            _userAccessor = userAccessor;
            allowedActiveGameSessions = _configuration.GetValue<int>("GeneralSettings:NumberOfConnectionsAllowed")!;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }


        //Improve this method! Handle how to close the game session and inform the user about it!
        public async Task<IResult> GetNextQuestion()
        {
            var user  = _userAccessor.UserName;
            var userData = await _authenticationService.GetUser(user);
            UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;

            if (userToDisplayDto == null)
            {
                return null;
            }

            var activeTable = await _activeGameSessionsRepository.GetActiveGameSession(userToDisplayDto.userId);
            string guid = activeTable.GameSessionId;

            if(guid == null)
            {
                return Results.Ok("Finish");
            }
    
            var question5ScoreIds = await _tempGameSessionRepository.GetQuestionOf5Score(guid);
            var question10ScoreIds = await _tempGameSessionRepository.GetQuestionOf10Score(guid);

            int[] questionIdsToArray;

            if(question5ScoreIds.Count() != 0)
            {
                questionIdsToArray = question5ScoreIds.ToArray();
            }
            else
            {
                questionIdsToArray = question10ScoreIds.ToArray();
            }

            if (questionIdsToArray.Length > 0)
            {
                Random random = new();
                int index = random.Next(0, questionIdsToArray.Length - 1);

                QuestionDto getQuestion = await _tempGameSessionRepository.GetQuestion(guid, questionIdsToArray[index]);

                return Results.Ok(getQuestion);
            }
            else
            {
                //await CloseGameSession(guid);
                return Results.Ok("Finish");
            }


        }

        public async Task<IResult> CheckForActiveGameSession()
        {
            var currentUser = _userAccessor.UserName;
            var activeGameSession = await _activeGameSessionsRepository.GetActiveGameSession(currentUser);

            if (activeGameSession != null) 
            {
                return Results.Ok(true);
            }
            else
            {
                return Results.Ok(false);
            }
        }

        //Might be optimized?
        public async Task<GameSessionDto> GetGameSession(int userRequestedQuestions = 10)
        {
            //check if a there are available spots
            int activeGameSessionsCount = await _activeGameSessionsRepository.GetActiveGameSessionCount();
            var currentUser = _userAccessor.UserName;

            if (activeGameSessionsCount >= allowedActiveGameSessions)
            {
                return new GameSessionDto()
                {
                    GameSessionId = $"There is already {allowedActiveGameSessions} active game sessions! Please try again later!",
                    UserName = "NotAllowerd",
                };
            }

            //check for game session
            var user = await _authenticationService.GetUser(currentUser);
            UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(user);

            var checkForGameSession = await _activeGameSessionsRepository.GetActiveGameSession(userToDisplayDto.userId);

            if (checkForGameSession != null)
            {
                return checkForGameSession;
            }

            //Establish connection to the sqlite database and check if the question count matches what has been requested
            using var connection = SqlConnection.CreateConnection(_connectionString);

            int questionsCount = await _questionRepository.GetQuestionCount();

            if (questionsCount < userRequestedQuestions)
            {
                throw new ApplicationException($"The questions count was less than {userRequestedQuestions}! The database must include at least {userRequestedQuestions} questions!");
            }
            else
            {
                string randomTableName = GenerateRandomTableName();

                var howManyQuestionsToSelect = $@"Drop TABLE IF EXISTS {randomTableName};
                                                Create Temp Table {randomTableName} AS
                                                SELECT   
	                                                 (SELECT CAST(count(*) AS REAL) FROM Questions) AS TotalQuestionCount,
                                                     (SELECT CAST(count(*) AS REAL) FROM Questions WHERE QuestionScore = 5) AS LowerScore,   
                                                     (SELECT CAST(count(*) AS REAL) FROM Questions WHERE QuestionScore = 10) AS HigherScore;
                                                SELECT 
	                                                (SELECT CAST(round(LowerScore / TotalQuestionCount, 1) * 10 AS INT)) AS LowerScorePercentage,
	                                                (SELECT CAST(round(HigherScore / TotalQuestionCount, 1) * 10 AS INT)) AS HigherScorePercentage  From {randomTableName} ;
                                                Drop TABLE {randomTableName};";

                var questionsOfDifferentScoreCount = await connection.QuerySingleOrDefaultAsync<QuestionsCount>(howManyQuestionsToSelect);

                var getLowScoreIdsSql = await _questionRepository.GetQuestion5Score();

                //var getLowScoreIdsSql = "Select QuestionId  from Questions where QuestionScore = 5";
               // var lowScoreIds = await connection.QueryAsync<int>(getLowScoreIdsSql);
                IEnumerable<int> LowScoreQuestions = GenerateRandomTable(getLowScoreIdsSql.ToArray(), questionsOfDifferentScoreCount!.LowerScorePercentage);


                var highScoreIds = await _questionRepository.GetQuestion10Score();
                //var getHighScoreIdsSql = "Select QuestionId  from Questions where QuestionScore = 10";
                //var highScoreIds = await connection.QueryAsync<int>(getHighScoreIdsSql);
                IEnumerable<int> HighScoreQuestions = GenerateRandomTable(highScoreIds.ToArray(), questionsOfDifferentScoreCount!.HigherScorePercentage);

                var questionIdsList = LowScoreQuestions.Concat(HighScoreQuestions).ToArray();

                string finalSql = CreateSelectQuestionsSqlStatement(questionIdsList);
                var questionsList = await connection.QueryAsync<Question>(finalSql);
                IEnumerable <Question> questionsListx2 = questionsList.ToList();

                //Current work
                var activeGameSessionObject = ConstructActiveGameSessionObject(userToDisplayDto.userId, userToDisplayDto.userName);

                //Post it to the database 
                var result = await _activeGameSessionsCommands.InsertActiveGameSession(activeGameSessionObject);

                //Create temp table and insert objects
                await _tempGameSessionCommands.CreateTempTable(activeGameSessionObject.GameSessionId);
                await _tempGameSessionCommands.InsertQuestions(questionsListx2, activeGameSessionObject.GameSessionId);


                return CreateGameSessionDto(activeGameSessionObject);
            }

        }

        public async Task<IResult> CheckCorrectAnswer(AnswerDto answerDto)
        {
            var currentUser = _userAccessor.UserName;
            var activeGameSession = await _activeGameSessionsRepository.GetActiveGameSession(currentUser);
            //Check if the answer was correct
            string correctAnswer = await _tempGameSessionRepository.FindCorrectAnswer(activeGameSession.GameSessionId, answerDto.QuestionId);

            if (correctAnswer == answerDto.Answer)
            {
                //Mark answer as correct in the database
                await _tempGameSessionCommands.PostAnswer(activeGameSession.GameSessionId, answerDto.QuestionId, 1);
                return Results.Ok("Correct");
            }
            else
            {
                //Mark answer as incorrect in the database
                await _tempGameSessionCommands.PostAnswer(activeGameSession.GameSessionId, answerDto.QuestionId, 0);
                return Results.Ok("Incorrect");
            }
        }

        public async Task<IResult> CloseGameSession()
        {
            var user = _userAccessor.UserName;
            var userData = await _authenticationService.GetUser(user);
            UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;

            if (userToDisplayDto == null)
            {
                return null;
            }

            var activeTable = await _activeGameSessionsRepository.GetActiveGameSession(userToDisplayDto.userId);
            string guid = activeTable.GameSessionId;


            //Get Game Session's Current Open Table 
            IEnumerable<QuestionsCaching> questionsToCache = await _tempGameSessionRepository.GetAll(guid);
            string questionsToCacheJson = JsonSerializer.Serialize(questionsToCache);

            //Get current game session
            GameSessionDto activeGameSession = await _activeGameSessionsRepository.GetActiveGameSessionByGuid(guid);

            CachedGameSessionModel cachedGameSessionModel = new()
            {
                GameSessionId = guid,
                UserId = activeGameSession.UserId, 
                UserName = activeGameSession.UserName, 
                Questions = questionsToCacheJson,
                SessionTime = activeGameSession.SessionTime,
            };

            //Insert it to the cached table
            await _cashedGameSessions.Insert(cachedGameSessionModel);

            //Drop Active game session and the temp table after caching
            await _tempGameSessionCommands.DropTempTable(guid);
            await _activeGameSessionsCommands.RemoveGameSession(guid);
            return Results.Ok("Game session has been terminated!");
        }

        private static ActiveGameSession ConstructActiveGameSessionObject(int userId, string userName)
        {
            ActiveGameSession activeGameSession = new()
            {
                UserName = userName,
                UserId = userId,
            };

            return activeGameSession;
        }

        private static string CreateSelectQuestionsSqlStatement(int[] questionIds)
        {
            string convertedIds = "";
            for (int i = 0; questionIds.Length > i; i++)
            {
                if (i == 0)
                {
                    convertedIds += $"({questionIds[i]},";
                }
                else if (i == questionIds.Length - 1)
                {
                    convertedIds += $"{questionIds[i]})";
                }
                else
                {
                    convertedIds += $"{questionIds[i]},";
                }
            }

            string sql = $"Select * from Questions where QuestionId IN {convertedIds}";
            return sql;
        }

        private static string GenerateRandomTableName()
        {
            Random random = new();
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            string randomTableName = "";

            for (int i = 0; i < 21; i++)
            {
                int x = random.Next(26);
                randomTableName = randomTableName + alphabet[x];
            }
            return randomTableName;
        }

        private static List<int> GenerateRandomTable(IEnumerable<int> Ids, int numbersToSelect)
        {
            int[] tempArray = Ids.ToArray();
            List<int> randomIds = new List<int>();

            Random random = new();
            for (int i = 1; numbersToSelect >= i; i++)
            {
                int randomIndex = random.Next(0, tempArray.Length);
                randomIds.Add(tempArray[randomIndex]);
                tempArray = tempArray.Where((source, index) => index != randomIndex).ToArray();

            }
            return randomIds;
        }

        private static GameSessionDto CreateGameSessionDto(ActiveGameSession activeGameSession)
        {
            GameSessionDto gameSessionDto = new()
            {
                GameSessionId = activeGameSession.GameSessionId,
                UserId = activeGameSession.UserId,
                UserName = activeGameSession.UserName,
                SessionTime = activeGameSession.SessionTime,
            };

            return gameSessionDto;
        }

    }
}
