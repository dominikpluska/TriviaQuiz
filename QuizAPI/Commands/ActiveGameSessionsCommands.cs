using Dapper;
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
            using var connection = CreateConnection();
            var sql = @"INSERT INTO ActiveGameSessions (
                                    GameSessionId, UserId, UserName,Questions, SessionTime) 
                                    VALUES 
                                    (@GameSessionId, @UserId, @UserName, @Questions, @SessionTime);
                                    ";
            await connection.ExecuteAsync(sql, activeGameSession);
            return Results.Ok("A new game session has been created!");

        }

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}
