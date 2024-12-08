using QuizAPI.Dto;

namespace QuizAPI.GameManager
{
    public interface IGameManager
    {
        public Task<GameSessionDto> GetGameSession(int userRequestedQuestions = 10);

        public Task<QuestionDto> GetNextQuestion(string guid);
    }
}
