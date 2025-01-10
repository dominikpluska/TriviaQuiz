using System.Security.Claims;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;

namespace AuthAPI.JwtGenerator
{
    public class CreateJwtToken : ICreateJwtToken
    {
        private readonly string _tokenString;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly DateTime _expiryTime = DateTime.Now.AddHours(1);
        private readonly IConfiguration _configuration;


        public CreateJwtToken(IConfiguration configuration)
        {
            _configuration = configuration;
            _tokenString = _configuration.GetValue<string>("JwtSettings:TokenString")!;
            _issuer = _configuration.GetValue<string>("JwtSettings:Issuer")!;
            _audience = _configuration.GetValue<string>("JwtSettings:Audience")!;

        }

        public string GenerateToken(string userName)
        {
            List<Claim> claims = new();
            claims.Add(new Claim(ClaimTypes.Name, userName));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: _expiryTime,
                signingCredentials: credentials
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }
}
