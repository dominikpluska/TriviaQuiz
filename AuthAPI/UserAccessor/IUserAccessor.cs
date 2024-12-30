namespace AuthAPI.UserAccessor
{
    public interface IUserAccessor
    {
        string UserName { get; }
        string TokenString { get; }

        void SetCookie(string cookieName, string data, CookieOptions cookieOptions);
    }
}
