using QuizAPI.Dto;

namespace QuizAPI.Repository
{
    public interface IGameSessionRepository
    {
        public Task<GameSessionDto> GetGameSession(int userRequestedQuestions = 10);
    }
}
