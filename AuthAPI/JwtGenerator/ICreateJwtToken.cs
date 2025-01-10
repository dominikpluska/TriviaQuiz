namespace AuthAPI.JwtGenerator
{
    public interface ICreateJwtToken
    {
        public string GenerateToken(string userName);
    }
}
