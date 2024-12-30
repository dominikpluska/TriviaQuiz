using QuizAPI.Dto;

namespace QuizAPI.GameManager
{
    public interface IGameManager
    {
        public Task<GameSessionDto> GetGameSession(HttpContext httpContext, int userRequestedQuestions = 10);

        public Task<QuestionDto> GetNextQuestion(HttpContext httpContext);
        public Task<string> CheckCorrectAnswer(AnswerDto answerDto);
    }
}
