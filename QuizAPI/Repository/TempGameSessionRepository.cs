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

                var sqlGetQuestion = $@"SELECT QuestionId, QuestionTitle, QuestionDescription, QuestionCategory, 
                                        OptionA, OptionB, OptionC, OptionD, QuestionScore FROM '{guid}'
                                        WHERE QuestionId={id} AND WasAnswerCorrect IS NULL";

                var executeGetQuestion = await connection.QueryAsync<QuestionDto>(sqlGetQuestion);

                return executeGetQuestion.FirstOrDefault();

        }

        public async Task<string> FindCorrectAnswer(string guid, int questionId)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"Select CorrectAnswer FROM '{guid}' where QuestionId = {questionId}";
            var result = await connection.QueryAsync<string>(sql);
            return result.FirstOrDefault();
        }
    }
}
