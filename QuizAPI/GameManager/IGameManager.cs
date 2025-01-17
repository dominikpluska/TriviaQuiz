﻿using Microsoft.AspNetCore.Mvc;
using QuizAPI.Dto;

namespace QuizAPI.GameManager
{
    public interface IGameManager
    {
        public Task<IResult> GetGameSession(int userRequestedQuestions = 10);
        public Task<IResult> GetNextQuestion();
        public Task<IResult> CheckCorrectAnswer(AnswerDto answerDto);
        public Task<IResult> CloseGameSession();
        public Task<IResult> CheckForActiveGameSession();
    }
}
