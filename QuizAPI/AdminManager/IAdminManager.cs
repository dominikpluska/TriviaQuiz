using QuizAPI.Dto;
using QuizAPI.Models;

namespace QuizAPI.AdminManager
{
    public interface IAdminManager
    {
        public Task<IResult> GetAllQuestions();
        public Task<IResult> GetQuestionDetails(int questionId);
        public Task<IResult> UpdateQuestion(int questionId, QuestionExtendedDto question);
        public Task<IResult> PostNewQuestion(QuestionExtendedDto question);
        public Task<IResult> DeleteQuestion(int questionId);
    }
}
