using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using QuizAPI.Dto;
using QuizAPI.HelperMethods;
using QuizAPI.Models;
using System;

namespace QuizAPI.Repository
{
    public class TempGameSessionRepository : ITempGameSessionRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public TempGameSessionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:GameSessions")!;
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

        public async Task<IEnumerable<QuestionsCaching>> GetAll(string guid)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"Select * FROM '{guid}'";
            var result = await connection.QueryAsync<QuestionsCaching>(sql);

            return result.ToList();
        }

        public async Task<IEnumerable<int>> GetNotAnsweredQuestions(string guid)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);

            var sql = @$"SELECT QuestionId 
                            FROM '{guid}'
                            WHERE QuestionScore = 5 AND WasAnswerCorrect IS NULL
                            AND (SELECT COUNT(*) FROM '{guid}' WHERE QuestionScore = 5) > 0
                            UNION ALL
                            SELECT QuestionId 
                            FROM '{guid}'
                            WHERE QuestionScore = 10 AND WasAnswerCorrect IS NULL
                            AND (SELECT COUNT(*) FROM '{guid}' WHERE QuestionScore = 5) = 0;";

            return await connection.QueryAsync<int>(sql);
        }

    }
}
