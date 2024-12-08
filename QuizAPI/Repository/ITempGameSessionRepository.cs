using QuizAPI.Dto;

namespace QuizAPI.Repository
{
    public interface ITempGameSessionRepository
    {
        public Task<QuestionDto> GetQuestion(string guid, int id);
        public Task<string> FindCorrectAnswer(string guid, int questionId);
    }
}
