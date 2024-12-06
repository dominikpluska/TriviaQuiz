using QuizAPI.Dto;

namespace QuizAPI.Repository
{
    public interface IActiveGameSessionsRepository
    {
        public Task<GameSessionDto> GetActiveGameSession(int id);
    }
}
