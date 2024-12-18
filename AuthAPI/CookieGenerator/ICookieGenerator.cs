namespace AuthAPI.CookieGenerator
{
    public interface ICookieGenerator
    {
        public CookieOptions GenerateCookie(DateTime dateTime);
    }
}
