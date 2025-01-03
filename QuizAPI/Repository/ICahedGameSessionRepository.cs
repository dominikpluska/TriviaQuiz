using QuizAPI.Models;

namespace QuizAPI.Repository
{
    public interface ICahedGameSessionRepository
    {
        public Task<CachedGameModel> GetLastGameStatisticsAsync(int userId);
        public Task<IEnumerable<CachedGameModelExtended>> GetListOfPlayedGames(int userId);
        public Task<CachedGameSessionModel> GetGameSessionStatistic(string gameSessionId);
    }
}
