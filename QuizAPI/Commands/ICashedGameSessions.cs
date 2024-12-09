using QuizAPI.Models;

namespace QuizAPI.Commands
{
    public interface ICashedGameSessions
    {
        public Task<IResult> Insert(CachedGameSessionModel cachedGameSession);
    }
}
