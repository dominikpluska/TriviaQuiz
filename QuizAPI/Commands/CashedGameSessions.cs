using Dapper;
using Microsoft.Extensions.Configuration;
using QuizAPI.HelperMethods;
using QuizAPI.Models;

namespace QuizAPI.Commands
{
    public class CashedGameSessions : ICashedGameSessions
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public CashedGameSessions(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<IResult> Insert(CachedGameSessionModel cachedGameSession)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"INSERT INTO CashedGameSessions (GameSessionId, UserId, UserName, Questions, SessionTime)
                         VALUES (@GameSessionId, @UserId, @UserName, @Questions, @SessionTime)";
            await connection.ExecuteAsync(sql, cachedGameSession);
            return Results.Ok("Game session has been cached!");
        }
    }
}
