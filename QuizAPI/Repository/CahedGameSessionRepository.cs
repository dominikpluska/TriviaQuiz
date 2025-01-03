using Dapper;
using QuizAPI.Dto;
using QuizAPI.HelperMethods;
using QuizAPI.Models;

namespace QuizAPI.Repository
{
    public class CahedGameSessionRepository : ICahedGameSessionRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public CahedGameSessionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<CachedGameModel> GetLastGameStatisticsAsync(int userId)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"SELECT Score, TotalQuestionCount, AnsweredQuestions from CachedGameSessions where UserId = {userId}
                         ORDER BY rowid DESC
                         LIMIT 1;";
            var resutls = await connection.QueryAsync<CachedGameModel>(sql);
            return resutls.FirstOrDefault();
        }
        public async Task<IEnumerable<CachedGameModelExtended>> GetListOfPlayedGames(int userId)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"Select GameSessionId, Score, TotalQuestionCount, AnsweredQuestions, SessionTime FROM CachedGameSessions
                        WHERE UserId = {userId} 
                        ORDER BY rowid DESC";

            var resutls = await connection.QueryAsync<CachedGameModelExtended>(sql);
            return resutls.ToList();
        }

        public async Task<CachedGameSessionModel> GetGameSessionStatistic(string gameSessionId)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"Select * FROM CachedGameSessions WHERE GameSessionId = '{gameSessionId}'";

            var results = await connection.QueryAsync<CachedGameSessionModel>(sql);
            return results.FirstOrDefault();
        }
    }
}
