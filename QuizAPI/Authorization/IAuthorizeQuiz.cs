using QuizAPI.Dto;

namespace QuizAPI.Authorization
{
    public interface IAuthorizeQuiz
    {
        public Task<UserToDisplayDto> GetUser();
        public Task<bool> IsGameMaster();
        public Task<bool> IsAuthorized();
    }
}
