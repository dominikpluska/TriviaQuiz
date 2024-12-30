namespace QuizAPI.UserAccessor
{
    public class HttpUserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _accessor;
        public HttpUserAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        string IUserAccessor.UserName => _accessor.HttpContext.Request.Cookies["TriviaQuizUserName"];

    }
}
