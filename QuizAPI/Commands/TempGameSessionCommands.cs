using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using QuizAPI.Dto;
using QuizAPI.HelperMethods;
using QuizAPI.Models;
using QuizAPI.Repository;
using System;

namespace QuizAPI.Commands
{
    public class TempGameSessionCommands : ITempGameSessionCommands
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IActiveGameSessionsRepository _activeGameSessionsRepository;

        public TempGameSessionCommands(IConfiguration configuration, IActiveGameSessionsRepository activeGameSessionsRepository)
        {
            _activeGameSessionsRepository = activeGameSessionsRepository;
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:GameSessions")!;
        }

        public async Task<IResult> CreateTempTable(string guid)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            string sql = $@"Create Table '{guid}'(
                            QuestionId INTEGER NOT NULL PRIMARY KEY,
                            QuestionTitle TEXT NOT NULL,
                            QuestionDescription TEXT NOT NULL,
                            QuestionCategory TEXT NOT NULL,
                            OptionA TEXT NOT NULL,
                            OptionB TEXT NOT NULL,
                            OptionC TEXT NOT NULL,
                            OptionD TEXT NOT NULL,
                            CorrectAnswer TEXT NOT NULL,
                            QuestionScore INTEGER NOT NULL,
                            WasAnswerCorrect INTEGER DEFAULT NULL)
                            ";
            await connection.ExecuteAsync(sql);
            return Results.Ok($"Temp table {guid} has been created!");
        }

        public async Task<IResult> InsertQuestions(IEnumerable<Question> questions, string guid)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);

            var sql = $@"INSERT INTO '{guid}'(
                                    QuestionId, QuestionTitle, QuestionDescription, QuestionCategory,
                                    OptionA, OptionB, OptionC, OptionD, CorrectAnswer, QuestionScore)
                                    VALUES 
                                    (@QuestionId, @QuestionTitle, @QuestionDescription, @QuestionCategory,
                                    @OptionA, @OptionB, @OptionC, @OptionD, @CorrectAnswer, @QuestionScore);
                                    ";
            await connection.ExecuteAsync(sql, questions);
            return Results.Ok($"Questions have been seeded!");
        }

        public async Task<IResult> DropTempTable(string guid)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"DROP TABLE {guid}";
            await connection.ExecuteAsync(sql);
            return Results.Ok($"Table '{guid}' has been dropped!");
        }

        public async Task<IResult> DropTempTables()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var activeGameSessions = await _activeGameSessionsRepository.GetAllGamesessionGuids();
            var activeGameSessionsArray = activeGameSessions.ToArray();

            if(activeGameSessionsArray.Length > 0)
            {
                foreach (var guidName in activeGameSessionsArray)
                {
                    var sql = $@"DROP TABLE '{guidName}'";
                    await connection.ExecuteAsync(sql);
                }
                return Results.Ok($"Temp tables have been removed!");
            }
            else
            {
                return Results.Ok($"No temp tables to clear");
            }

        }

        public async Task<IResult> PostAnswer(string guid,int questionId ,int wasCorrect)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @$"UPDATE '{guid}'SET WasAnswerCorrect ={wasCorrect}
                            WHERE QuestionId = {questionId}";
            await connection.ExecuteAsync(sql);
            return Results.Ok("Table updated!");
        }

    }
}
