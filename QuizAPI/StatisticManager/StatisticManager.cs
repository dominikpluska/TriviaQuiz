﻿using QuizAPI.Dto;
using QuizAPI.Models;
using QuizAPI.Repository;
using QuizAPI.Services;
using QuizAPI.UserAccessor;
using System.Text.Json;

namespace QuizAPI.StatisticManager
{
    public class StatisticManager : IStatisticManager
    {
        private readonly IUserAccessor _userAccessor;
        private readonly ICahedGameSessionRepository _cahedGameSessionRepository;
        private readonly IAuthenticationService _authenticationService;
        public StatisticManager(IUserAccessor userAccessor, ICahedGameSessionRepository cahedGameSessionRepository,
            IAuthenticationService authenticationService)
        {
            _userAccessor = userAccessor;
            _authenticationService = authenticationService;
            _cahedGameSessionRepository = cahedGameSessionRepository;
        } 

        public async Task<IResult> GetLastPlayedGame()
        {
            var user = _userAccessor.UserName;
            var userData = await _authenticationService.GetUser(user);
            UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;

            var lastPlayedGame = await _cahedGameSessionRepository.GetLastGameStatisticsAsync(userToDisplayDto.userId);

            return Results.Ok(lastPlayedGame);

        }

        public async Task<IResult> GetAllPlayedGames()
        {
            var user = _userAccessor.UserName;
            var userData = await _authenticationService.GetUser(user);
            UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;

            var gamesList = await _cahedGameSessionRepository.GetListOfPlayedGames(userToDisplayDto.userId);

            return Results.Ok(gamesList);

        }

        public async Task<IResult> GetGameSessionStats(string gamesessionId)
        {
            if(gamesessionId == null || gamesessionId == string.Empty)
            {
                return Results.BadRequest("GameSessionId is empty!");
            }
            var user = _userAccessor.UserName;
            var userData = await _authenticationService.GetUser(user);
            UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;

            var game = await _cahedGameSessionRepository.GetGameSessionStatistic(gamesessionId);
            CachedGameSessionDto cachedGameSessionDto = new()
            {
                TotalQuestionCount = game.TotalQuestionCount,
                AnsweredQuestions = game.AnsweredQuestions,
                GameSessionId = game.GameSessionId,
                Score = game.Score,
                SessionTime = game.SessionTime,
                Questions = JsonSerializer.Deserialize<IEnumerable<QuestionsCaching>>(game.Questions)!
            };

            if (game.UserId == userToDisplayDto.userId)
            {
                return Results.Ok(cachedGameSessionDto);
            }
            else
            {
                return Results.Unauthorized();
            }
        }
    }
}