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

        public async Task<Question> GetQuestion(int id)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @$"SELECT * FROM Questions WHERE QuestionId = {id}";
            var resutls = await connection.QuerySingleAsync<Question>(sql);

            return resutls;
        }
    }
}
