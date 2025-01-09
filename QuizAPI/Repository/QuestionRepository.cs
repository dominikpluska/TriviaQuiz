using Dapper;
using QuizAPI.Dto;
using QuizAPI.HelperMethods;
using QuizAPI.Models;
using System;
using System.Data;
using System.Data.SQLite;


namespace QuizAPI.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public QuestionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<IEnumerable<Question>> GetAllQuestions()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);

            var sql = @"SELECT * FROM Questions";

            var resutls = await connection.QueryAsync<Question>(sql);

            return resutls.ToList();
        }

        public async Task<IEnumerable<QuestionLightModelDto>> GetAllQuestionsLight()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);

            var sql = @"SELECT QuestionId,QuestionTitle,QuestionCategory,QuestionScore,CorrectAnswer 
                    FROM Questions";

            var resutls = await connection.QueryAsync<QuestionLightModelDto>(sql);

            return resutls.ToList();
        }

        public async Task<Question> GetQuestion(int id)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @$"SELECT * FROM Questions WHERE QuestionId = {id}";
            var resutls = await connection.QuerySingleAsync<Question>(sql);

            return resutls;
        }

        public async Task<int> GetQuestionCount()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sqlCheckCount = "Select count(*) from Questions";
            return await connection.ExecuteScalarAsync<int>(sqlCheckCount);
        }

        public async Task<IEnumerable<int>> GetQuestion5Score()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var getLowScoreIdsSql = "Select QuestionId  from Questions where QuestionScore = 5";
            return await connection.QueryAsync<int>(getLowScoreIdsSql);
        }

        public async Task<IEnumerable<int>> GetQuestion10Score()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var getLowScoreIdsSql = "Select QuestionId  from Questions where QuestionScore = 10";
            return await connection.QueryAsync<int>(getLowScoreIdsSql);
        }
    }
}
