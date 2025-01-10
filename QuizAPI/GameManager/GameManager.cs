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
        }

        public async Task<IResult> GetNextQuestion()
        {
            try
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

                if (guid == null)
                {
                    return Results.Ok("Finish");
                }

                var question5ScoreIds = await _tempGameSessionRepository.GetQuestionOf5Score(guid);
                var question10ScoreIds = await _tempGameSessionRepository.GetQuestionOf10Score(guid);

                int[] questionIdsToArray;

                if (question5ScoreIds.Count() != 0)
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
                    await CacheGameSession(guid);
                    return Results.Ok("Finish");
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }

        }

        public async Task<IResult> CheckForActiveGameSession()
        {
            try
            {
                var currentUser = _userAccessor.UserName;
                var userData = await _authenticationService.GetUser(currentUser);
                UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;

                var activeGameSession = await _activeGameSessionsRepository.GetActiveGameSession(userToDisplayDto.userId);

                if (activeGameSession != null)
                {
                    return Results.Ok(true);
                }
                else
                {
                    return Results.Ok(false);
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        //Might be optimized?
        public async Task<IResult> GetGameSession(int userRequestedQuestions = 10)
        {
            try 
            {
                //check if a there are available spots
                int activeGameSessionsCount = await _activeGameSessionsRepository.GetActiveGameSessionCount();
                var currentUser = _userAccessor.UserName;

                if (activeGameSessionsCount >= allowedActiveGameSessions)
                {
                    return Results.Problem($"There is already {allowedActiveGameSessions} active game sessions! Please try again later!");
                }

                //check for game session
                var user = await _authenticationService.GetUser(currentUser);
                UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(user);

                var checkForGameSession = await _activeGameSessionsRepository.GetActiveGameSession(userToDisplayDto.userId);

                if (checkForGameSession != null)
                {
                    return Results.Ok(checkForGameSession);
                }

                int questionsCount = await _questionRepository.GetQuestionCount();

                if (questionsCount < userRequestedQuestions)
                {
                    throw new ApplicationException($"The questions count was less than {userRequestedQuestions}! The database must include at least {userRequestedQuestions} questions!");
                }
                else
                {
                    string randomTableName = GenerateRandomTableName();
                    var questionCount = await _questionRepository.CalculateQuestionPercentage(randomTableName, userRequestedQuestions);
                    var getLowScoreIdsSql = await _questionRepository.GetQuestion5Score();
                    IEnumerable<int> LowScoreQuestions = GenerateRandomTable(getLowScoreIdsSql.ToArray(), questionCount!.LowerScorePercentage);

                    var highScoreIds = await _questionRepository.GetQuestion10Score();
                    IEnumerable<int> HighScoreQuestions = GenerateRandomTable(highScoreIds.ToArray(), questionCount!.HigherScorePercentage);

                    var questionIdsList = LowScoreQuestions.Concat(HighScoreQuestions).ToArray();
                    var questionsList = await _questionRepository.GetQuestionsForQuiz(questionIdsList);

                    var activeGameSessionObject = ConstructActiveGameSessionObject(userToDisplayDto.userId);
                    await _activeGameSessionsCommands.InsertActiveGameSession(activeGameSessionObject);

                    await _tempGameSessionCommands.CreateTempTable(activeGameSessionObject.GameSessionId);
                    await _tempGameSessionCommands.InsertQuestions(questionsList, activeGameSessionObject.GameSessionId);

                    return Results.Ok(CreateGameSessionDto(activeGameSessionObject));
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        public async Task<IResult> CheckCorrectAnswer(AnswerDto answerDto)
        {
            try
            {
                var currentUser = _userAccessor.UserName;
                var userData = await _authenticationService.GetUser(currentUser);
                UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;

                var activeGameSession = await _activeGameSessionsRepository.GetActiveGameSession(userToDisplayDto.userId);
                string correctAnswer = await _tempGameSessionRepository.FindCorrectAnswer(activeGameSession.GameSessionId, answerDto.QuestionId);

                if (correctAnswer == answerDto.Answer)
                {
                    await _tempGameSessionCommands.PostAnswer(activeGameSession.GameSessionId, answerDto.QuestionId, 1);
                    return Results.Ok("Correct");
                }
                else
                {
                    await _tempGameSessionCommands.PostAnswer(activeGameSession.GameSessionId, answerDto.QuestionId, 0);
                    return Results.Ok("Incorrect");
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        public async Task<IResult> CloseGameSession()
        {
            try
            {
                var user = _userAccessor.UserName;
                var userData = await _authenticationService.GetUser(user);
                UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;

                if (userToDisplayDto == null)
                {
                    return Results.Problem("User was empty!");
                }

                var activeTable = await _activeGameSessionsRepository.GetActiveGameSession(userToDisplayDto.userId);
                string guid = activeTable.GameSessionId;
                //Drop Active game session and the temp table
                await _tempGameSessionCommands.DropTempTable(guid);
                await _activeGameSessionsCommands.RemoveGameSession(guid);
                return Results.Ok("Game session has been terminated!");
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        private static ActiveGameSession ConstructActiveGameSessionObject(int userId)
        {
            ActiveGameSession activeGameSession = new()
            {
                UserId = userId,
            };

            return activeGameSession;
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

        private async Task CacheGameSession(string guid)
        {
            IEnumerable<QuestionsCaching> questionsToCache = await _tempGameSessionRepository.GetAll(guid);
            string questionsToCacheJson = JsonSerializer.Serialize(questionsToCache);

            GameSessionDto activeGameSession = await _activeGameSessionsRepository.GetActiveGameSessionByGuid(guid);
            CachedGameSessionModel cachedGameSessionModel = new()
            {
                GameSessionId = guid,
                UserId = activeGameSession.UserId,
                Questions = questionsToCacheJson,
                Score = CalculateScore(questionsToCache),
                TotalQuestionCount = CalculateNumberOfTotalQuestions(questionsToCacheJson),
                AnsweredQuestions = CalculateNumberOfCorrectAnswers(questionsToCacheJson),
                SessionTime = activeGameSession.SessionTime,
            };

            await _cashedGameSessions.Insert(cachedGameSessionModel);
        }

        private static int CalculateScore(IEnumerable<QuestionsCaching> questionsCaching)
        {
            var sumOfCorrectAnswers = questionsCaching.Where(x => x.WasAnswerCorrect == 1).Select(x => x.QuestionScore).Sum();
            return sumOfCorrectAnswers;
        }
        private static int CalculateNumberOfTotalQuestions(string jsonString)
        {
            var deserializeQuestions = JsonSerializer.Deserialize<IEnumerable<QuestionsCaching>>(jsonString);
            var numberOfCorrectAnswers = deserializeQuestions!.Select(x => x.QuestionId).ToArray();
            return numberOfCorrectAnswers.Length;
        }

        private static int CalculateNumberOfCorrectAnswers(string jsonString)
        {
            var deserializeQuestions = JsonSerializer.Deserialize<IEnumerable<QuestionsCaching>>(jsonString);
            var numberOfCorrectAnswers = deserializeQuestions!.Where(x => x.WasAnswerCorrect == 1).Select(x => x.QuestionId).ToArray();
            return numberOfCorrectAnswers.Length;
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
                SessionTime = activeGameSession.SessionTime,
            };

            return gameSessionDto;
        }

    }
}
