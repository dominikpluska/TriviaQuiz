namespace AuthAPI.UserAccessor
{
    public class HttpUserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _accessor;
        public HttpUserAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string TokenString => _accessor.HttpContext.Request.Cookies["TriviaQuiz"];

        public string UserName => _accessor.HttpContext.Request.Cookies["TriviaQuizUserName"];

        public void SetCookie(string cookieName, string data, CookieOptions cookieOptions)
        {
            _accessor.HttpContext.Response.Cookies.Append(cookieName, data , cookieOptions);
        }
    }
}
