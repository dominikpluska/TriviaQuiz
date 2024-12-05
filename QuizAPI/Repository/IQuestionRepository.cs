using QuizAPI.Dto;
using QuizAPI.Models;

namespace QuizAPI.Repository
{
    public interface IQuestionRepository
    {
        public Task<Question> GetQuestion(int id);
        public Task<IEnumerable<Question>> GetAllQuestions();
        public Task RequestGameSession(int questionCount = 10);
    }
}
