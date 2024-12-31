using Dapper;
using QuizAPI.HelperMethods;
using QuizAPI.Models;
using System.Data.SQLite;

namespace QuizAPI.Commands
{
    public class ActiveGameSessionsCommands : IActiveGameSessionsCommands
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public ActiveGameSessionsCommands(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<IResult> InsertActiveGameSession(ActiveGameSession activeGameSession)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @"INSERT INTO ActiveGameSessions (
                                    GameSessionId, UserId, UserName, SessionTime) 
                                    VALUES 
                                    (@GameSessionId, @UserId, @UserName, @SessionTime);";
            var result = await connection.ExecuteAsync(sql, activeGameSession);
            return Results.Ok(result);

        }

        public async Task<IResult> TruncateActiveGameSession()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @"DELETE FROM ActiveGameSessions";
            await connection.ExecuteAsync(sql);
            return Results.Ok();
        }

        public async Task<IResult> RemoveGameSession(string activeGameSessionId)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @$"DELETE FROM ActiveGameSessions where GameSessionId = '{activeGameSessionId}'";
            await connection.ExecuteAsync(sql);
            return Results.Ok();
        }

    }
}
