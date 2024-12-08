using Dapper;
using Microsoft.Extensions.Configuration;
using QuizAPI.Dto;
using QuizAPI.HelperMethods;

namespace QuizAPI.Repository
{
    public class TempGameSessionRepository : ITempGameSessionRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public TempGameSessionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<QuestionDto> GetQuestion(string guid, int id)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            //var sql = @$"SELECT QuestionId 
            //                FROM '{guid}'
            //                WHERE QuestionScore = 5
            //                AND (SELECT COUNT(*) FROM Questions WHERE QuestionScore = 5) > 0
            //                UNION ALL
            //                SELECT QuestionId 
            //                FROM '{guid}'
            //                WHERE QuestionScore = 10
            //                AND (SELECT COUNT(*) FROM Questions WHERE QuestionScore = 5) = 0;";

            //var executeCommand = await connection.QueryAsync<int>(sql);
            //var result = executeCommand.ToArray();

            //if(result.Length > 0)
            //{
                //Random random = new();
                //int index = random.Next(0, result.Length - 1);

                var sqlGetQuestion = $@"SELECT QuestionId, QuestionTitle, QuestionDescription, QuestionCategory, 
                                        OptionA, OptionB, OptionC, OptionD, QuestionScore FROM '{guid}'
                                        WHERE QuestionId={id}";

                var executeGetQuestion = await connection.QueryAsync<QuestionDto>(sqlGetQuestion);

                return executeGetQuestion.FirstOrDefault();
            //}

        }
    }
}
