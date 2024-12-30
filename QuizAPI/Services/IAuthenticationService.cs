namespace QuizAPI.Services
{
    public interface IAuthenticationService
    {
        public Task Authorize(HttpContext httpContext);
        public Task<string> GetUser(string userName);
    }
}
