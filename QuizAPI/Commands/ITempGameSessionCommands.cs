using QuizAPI.Models;

namespace QuizAPI.Commands
{
    public interface ITempGameSessionCommands
    {
        public Task<IResult> CreateTempTable(string guid);
        public Task<IResult> InsertQuestions(IEnumerable<Question> questions, string guid);
        public Task<IResult> DropTempTable(string guid);
        public Task<IResult> DropTempTables();
        public Task<IResult> PostAnswer(string guid, int questionId, int wasCorrect);
    }
}
