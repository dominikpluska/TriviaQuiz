using QuizAPI.Dto;

namespace QuizAPI.Repository
{
    public interface IActiveGameSessionsRepository
    {
        public Task<GameSessionDto> GetActiveGameSession(int id);
        public Task<GameSessionDto> GetActiveGameSession(string guid);
        public Task<int> GetActiveGameSessionCount();
        public Task<IEnumerable<int>> GetActiveQuestions(string guid);
    }
}
