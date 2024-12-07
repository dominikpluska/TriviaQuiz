using QuizAPI.Models;

namespace QuizAPI.Commands
{
    public interface IActiveGameSessionsCommands
    {
        public Task<IResult> InsertActiveGameSession(ActiveGameSession activeGameSession);
        public Task<IResult> TruncateActiveGameSession();
        public Task<IResult> RemoveGameSession(string activeGameSessionId);
    }
}
