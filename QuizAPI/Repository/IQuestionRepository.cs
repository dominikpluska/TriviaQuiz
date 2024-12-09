using QuizAPI.Dto;
using QuizAPI.Models;

namespace QuizAPI.Repository
{
    public interface IQuestionRepository
    {
        public Task<Question> GetQuestion(int id);
        public Task<IEnumerable<Question>> GetAllQuestions();
        public Task<int> GetQuestionCount();
        public Task<IEnumerable<int>> GetQuestion5Score();

        public Task<IEnumerable<int>> GetQuestion10Score();
    }
}
