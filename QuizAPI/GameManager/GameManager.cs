using Dapper;
using Microsoft.AspNetCore.Components;
using QuizAPI.Commands;
using QuizAPI.Dto;
using QuizAPI.HelperMethods;
using QuizAPI.Models;
using QuizAPI.Repository;
using System.Data.SQLite;
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

        public GameManager(IConfiguration configuration, IActiveGameSessionsCommands activeGameSessionsCommands, IActiveGameSessionsRepository activeGameSessionsRepository,
                           ITempGameSessionCommands tempGameSessionCommands, ITempGameSessionRepository tempGameSessionRepository, ICashedGameSessions cashedGameSessions,
                           IQuestionRepository questionRepository)
        {
            _configuration = configuration;
            _activeGameSessionsCommands = activeGameSessionsCommands;
            _activeGameSessionsRepository = activeGameSessionsRepository;
            _tempGameSessionCommands = tempGameSessionCommands;
            _tempGameSessionRepository = tempGameSessionRepository;
            _cashedGameSessions = cashedGameSessions;
            _questionRepository = questionRepository;
            allowedActiveGameSessions = _configuration.GetValue<int>("GeneralSettings:NumberOfConnectionsAllowed")!;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<QuestionDto> GetNextQuestion(string guid)
        {
            var questionIds = await _tempGameSessionRepository.GetNotAnsweredQuestions(guid);
            var questionIdsToArray = questionIds.ToArray();

            if (questionIdsToArray.Length > 0)
            {
                Random random = new();
                int index = random.Next(0, questionIdsToArray.Length - 1);

                var getQuestion = await _tempGameSessionRepository.GetQuestion(guid, questionIdsToArray[index]);
                return getQuestion;
            }

            else
            {
                await CloseGameSession(guid);
                return null;
            }
        }

        //Might be optimized?
        public async Task<GameSessionDto> GetGameSession(int userRequestedQuestions = 10)
        {
            //check if a there are available spots
            int activeGameSessionsCount = await _activeGameSessionsRepository.GetActiveGameSessionCount();

            if(activeGameSessionsCount >= allowedActiveGameSessions)
            {
                return new GameSessionDto()
                {
                    GameSessionId = $"There is already {allowedActiveGameSessions} active game sessions! Please try again later!",
                    UserName = "NotAllowerd",
                };
            }

            //check for game session
            var checkForGameSession = await _activeGameSessionsRepository.GetActiveGameSession(1);

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

                var activeGameSessionObject = ConstructActiveGameSessionObject();

                //Post it to the database 
                var result = await _activeGameSessionsCommands.InsertActiveGameSession(activeGameSessionObject);

                //Create temp table and insert objects
                await _tempGameSessionCommands.CreateTempTable(activeGameSessionObject.GameSessionId);
                await _tempGameSessionCommands.InsertQuestions(questionsListx2, activeGameSessionObject.GameSessionId);


                return CreateGameSessionDto(activeGameSessionObject);
            }

        }

        //Update this method!
        public async Task<string> CheckCorrectAnswer(AnswerDto answerDto)
        {
            //Check if the answer was correct
            string answer = await _tempGameSessionRepository.FindCorrectAnswer(answerDto.Guid, answerDto.QuestionId);

            if (answer == answerDto.Answer)
            {
                //Mark answer as correct in the database
                await _tempGameSessionCommands.PostAnswer(answerDto.Guid, answerDto.QuestionId, 1);
                return "Correct";
            }
            else
            {
                //Mark answer as incorrect in the database
                await _tempGameSessionCommands.PostAnswer(answerDto.Guid, answerDto.QuestionId, 0);
                return "Incorrect";
            }
        }

        public async Task CloseGameSession(string guid)
        {
            //Get Game Session's Current Open Table 
            IEnumerable<QuestionsCaching> questionsToCache = await _tempGameSessionRepository.GetAll(guid);
            string questionsToCacheJson = JsonSerializer.Serialize(questionsToCache);

            //Get current game session
            GameSessionDto activeGameSession = await _activeGameSessionsRepository.GetActiveGameSession(guid);

            CachedGameSessionModel cachedGameSessionModel = new()
            {
                GameSessionId = guid,
                UserId = activeGameSession.UserId, 
                UserName = activeGameSession.UserName, 
                questions = questionsToCacheJson,
                SessionTime = activeGameSession.SessionTime,
            };

            //Insert it to the cached table
            await _cashedGameSessions.Insert(cachedGameSessionModel);

            //Drop Active game session and the temp table after caching
            await _activeGameSessionsCommands.RemoveGameSession(guid);
        }

        //This method and its content must be changed! 
        private static ActiveGameSession ConstructActiveGameSessionObject()
        {
            ActiveGameSession activeGameSession = new()
            {
                UserName = "Test",
                UserId = 1,
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
