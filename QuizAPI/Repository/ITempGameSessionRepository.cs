using QuizAPI.Dto;
using QuizAPI.Models;

namespace QuizAPI.Repository
{
    public interface ITempGameSessionRepository
    {
        public Task<QuestionDto> GetQuestion(string guid, int id);
        public Task<string> FindCorrectAnswer(string guid, int questionId);
        public Task<IEnumerable<QuestionsCaching>> GetAll(string guid);
    }
}
