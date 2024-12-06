using QuizAPI.Models;

namespace QuizAPI.Commands
{
    public interface IActiveGameSessionsCommands
    {
        public Task<IResult> InsertActiveGameSession(ActiveGameSession activeGameSession);
    }
}
