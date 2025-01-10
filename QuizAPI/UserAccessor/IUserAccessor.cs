namespace QuizAPI.UserAccessor
{
    public interface IUserAccessor
    {
        string UserName { get; }
        string Token { get; }
    }
}
