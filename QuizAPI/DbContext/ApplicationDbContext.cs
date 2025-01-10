using Microsoft.Extensions.Configuration;
using System.Data.SQLite;
using System.Data;
using Dapper;
using QuizAPI.Models;
using QuizAPI.HelperMethods;
using System.Text.Json;

namespace QuizAPI.DbContext
{
    public class ApplicationDbContext
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task CreateModel()
        {
            if (!File.Exists($"{Environment.CurrentDirectory}\\Quiz.db"))
            {
                SQLiteConnection.CreateFile($"{Environment.CurrentDirectory}\\Quiz.db");
            }
            if (!File.Exists($"{Environment.CurrentDirectory}\\Quiz.db"))
            {
                SQLiteConnection.CreateFile($"{Environment.CurrentDirectory}\\GameSessions.db");
            }

            await CreateTables();
            await SeedTables();
        }

        private async Task SeedTables()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);

            var sql_check = @"select count(*) from Questions";
            var check_result =  await connection.ExecuteScalarAsync<int>(sql_check);

            if (check_result < 10)
            {
                var sql_insert_questions = @"INSERT INTO Questions (
                                    QuestionTitle, QuestionDescription, QuestionCategory,
                                    OptionA, OptionB, OptionC, OptionD, CorrectAnswer, QuestionScore)
                                    VALUES 
                                    (@QuestionTitle, @QuestionDescription, @QuestionCategory,
                                    @OptionA, @OptionB, @OptionC, @OptionD, @CorrectAnswer, @QuestionScore);
                                    ";

                var questionList = ReadQuestionsFromJsonFile();
                await connection.ExecuteAsync(sql_insert_questions, questionList);
            }
        }

        private async Task CreateTables()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @"
                CREATE TABLE IF NOT EXISTS 
                Questions (
                    QuestionId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    QuestionTitle TEXT NOT NULL,
                    QuestionDescription TEXT NOT NULL,
                    QuestionCategory TEXT NOT NULL,
                    OptionA TEXT NOT NULL,
                    OptionB TEXT NOT NULL,
                    OptionC TEXT NOT NULL,
                    OptionD TEXT NOT NULL,
                    CorrectAnswer TEXT NOT NULL,
                    QuestionScore INTEGER NOT NULL
                );
                CREATE TABLE IF NOT EXISTS 
                QuizResults (
                    ResultId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    GameSessionId TEXT NOT NULL,
                    UserId INT NOT NULL,
                    GameResults TEXT NOT NULL
                    );
               CREATE TABLE IF NOT EXISTS 
               ActiveGameSessions(
                    GameSessionId TEXT NOT NULL UNIQUE,
                    UserId INT NOT NULL, 
                    SessionTime TEXT NOT NULL
                );
               CREATE TABLE IF NOT EXISTS 
               CachedGameSessions(
                    GameSessionId TEXT NOT NULL UNIQUE,
                    UserId INT NOT NULL, 
                    Questions TEXT NOT NULL,
                    Score INT NOT NULL,
                    AnsweredQuestions INT NOT NULL,
                    TotalQuestionCount INT NOT NULL,
                    SessionTime TEXT NOT NULL
                );
            ";

            await connection.ExecuteAsync(sql);
        }

        private IEnumerable<Question> ReadQuestionsFromJsonFile()
        {
            var list = _configuration.GetSection("InitialQuestionsList").Get<IEnumerable<Question>>()!;
            return list;
        }
    }
}
