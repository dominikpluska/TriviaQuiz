using AuthAPI.HelperMethods;
using AuthAPI.Models;
using Dapper;

namespace AuthAPI.Commands
{
    public class JwtCommands : IJwtCommands
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        public JwtCommands(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<IResult> Insert(Jwt jwt)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"INSERT INTO Jwt (UserId, Token) 
                        VALUES (@UserId, @Token)";
            await connection.ExecuteAsync(sql, jwt);

            return Results.Ok("A new record has been created!");
        }
    }
}
