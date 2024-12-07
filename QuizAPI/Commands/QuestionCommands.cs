using Dapper;
using QuizAPI.Models;
using System.Data;
using System.Data.SQLite;
using Microsoft.AspNetCore.Http;
using QuizAPI.HelperMethods;

namespace QuizAPI.Commands
{
    public class QuestionCommands : IQuestionCommands
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public QuestionCommands(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<IResult> Delete(int questionId)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);

            var sqlCheckIfExist = $@"Select Count(*) from Questions where QuestionId = {questionId}";
            int entity = await connection.ExecuteScalarAsync<int>(sqlCheckIfExist);

            if (entity != 0)
            {
                var sql = @$"Delete from Questions where QuestionId = {questionId}";
                await connection.ExecuteAsync(sql);
                return Results.Ok($"Entry {questionId} has been deleted!");

            }
            else if (entity == 0)
            {
                return Results.BadRequest($"Record not found!");
            }
            else
            {
                throw new InvalidOperationException("Error while processing the request!");
            }
        }

        public async Task<IResult> Insert(Question question)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @"INSERT INTO Questions (
                                    QuestionTitle, QuestionDescription, QuestionCategory,
                                    OptionA, OptionB, OptionC, OptionD, CorrectAnswer, QuestionScore) 
                                    VALUES 
                                    (@QuestionTitle, @QuestionDescription, @QuestionCategory,
                                    @OptionA, @OptionB, @OptionC, @OptionD, @CorrectAnswer, @QuestionScore);
                                    ";
            await connection.ExecuteAsync(sql, question);
            return Results.Ok("A new record has been created!");

        }

        public async Task<IResult> Update(Question question)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);

            var sql = $@"UPDATE Questions set QuestionTitle = (@QuestionTitle), QuestionDescription = (@QuestionDescription), QuestionCategory = (@QuestionCategory),
                        OptionA = (@OptionA), OptionB = (@OptionB), OptionC = (@OptionC), OptionD = (@OptionD), CorrectAnswer = (@CorrectAnswer), QuestionScore = @QuestionScore 
                        where QuestionId = {question.QuestionId}";

            await connection.ExecuteAsync(sql, question);

            return Results.Ok("Record has been updated!");
        }

    }
}
