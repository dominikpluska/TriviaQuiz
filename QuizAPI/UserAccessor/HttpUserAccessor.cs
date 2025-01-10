namespace QuizAPI.UserAccessor
{
    public class HttpUserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _accessor;
        public HttpUserAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Token => _accessor.HttpContext!.Request.Cookies["TriviaQuiz"]!;
        string IUserAccessor.UserName => _accessor.HttpContext!.Request.Cookies["TriviaQuizUserName"]!;

    }
}
