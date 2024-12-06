using Microsoft.Extensions.Configuration;
using System.Data.SQLite;
using System.Data;
using Dapper;
using QuizAPI.Models;

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

        public IDbConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public async Task CreateModel()
        {
            if (!File.Exists($"{Environment.CurrentDirectory}\\Quiz.db"))
            {
                SQLiteConnection.CreateFile($"{Environment.CurrentDirectory}\\Quiz.db");
            }

            await CreateTables();
            await SeedTables();
        }

        private async Task SeedTables()
        {
            using var connection = CreateConnection();

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

                IEnumerable<Question> questions = new List<Question>
                {
                    new Question() { QuestionTitle = "Poland's Capital", QuestionDescription = "Which city is Poland's capital?", QuestionCategory = "Geography",
                                     OptionA = "Poznań", OptionB = "Warszawa", OptionC = "Łódź", OptionD = "Gdańsk", CorrectAnswer = "Warszawa", QuestionScore = 5},
                    new Question() { QuestionTitle = "Smallest European Bird", QuestionDescription = "What is the smallest European bird?", QuestionCategory = "Biology",
                                     OptionA = "Tit", OptionB = "European robin", OptionC = "Goldcrest", OptionD = "House sparrow", CorrectAnswer = "Goldcrest", QuestionScore = 10},
                    new Question() { QuestionTitle = "Tallest European peak", QuestionDescription = "What is the tallest European peak?", QuestionCategory = "Geography",
                                     OptionA = "Mont Blanc", OptionB = "Shkhara", OptionC = "Mount Elbrus", OptionD = "The Matterhorn", CorrectAnswer = "Mount Elbrus", QuestionScore = 10},
                    new Question() { QuestionTitle = "Estonia's Capital", QuestionDescription = "Which city is Estonia's capital?", QuestionCategory = "Geography",
                                     OptionA = "Tallinn", OptionB = "Riga", OptionC = "Vilnius", OptionD = "Tartu", CorrectAnswer = "Tallinn", QuestionScore = 5},
                    new Question() { QuestionTitle = "Europe's longest river", QuestionDescription = "What is Europe's longest river?", QuestionCategory = "Geography",
                                     OptionA = "The Danube", OptionB = "The Volga", OptionC = "The Dnipro", OptionD = "The Belaya", CorrectAnswer = "The Volga", QuestionScore = 10},
                    new Question() { QuestionTitle = "Bjarne Stroustrup", QuestionDescription = "Bjarne Stroustrup is a creator of which programming language?", QuestionCategory = "Computer Science",
                                     OptionA = "Python", OptionB = "Golang", OptionC = "C++", OptionD = "Rust", CorrectAnswer = "C++", QuestionScore = 10},
                    new Question() { QuestionTitle = "Guido van Rossum", QuestionDescription = "Guido van Rossum is a creator of which programming language?", QuestionCategory = "Computer Science",
                                     OptionA = "Python", OptionB = "Golang", OptionC = "C++", OptionD = "Rust", CorrectAnswer = "Python", QuestionScore = 10},
                    new Question() { QuestionTitle = "JavaScript", QuestionDescription = "What was the original name of JavaScript", QuestionCategory = "Computer Science",
                                     OptionA = "Lua", OptionB = "Go", OptionC = "Matlab", OptionD = "Mocha", CorrectAnswer = "Mocha", QuestionScore = 10},
                    new Question() { QuestionTitle = "Game Engines", QuestionDescription = "Which Game Engine Uses C# as its programming language?", QuestionCategory = "Computer Science",
                                     OptionA = "Unity", OptionB = "Unreal Engine", OptionC = "CryEngine", OptionD = "RPGMaker", CorrectAnswer = "Unity", QuestionScore = 5},
                    new Question() { QuestionTitle = "Oiseau", QuestionDescription = "What does \"Oiseau\" mean in French?", QuestionCategory = "Languages",
                                     OptionA = "Fox", OptionB = "Bull", OptionC = "Bear", OptionD = "Bird", CorrectAnswer = "Bird", QuestionScore = 5},
                };
                await connection.ExecuteAsync(sql_insert_questions, questions);
            }
        }

        private async Task CreateTables()
        {
            using var connection = CreateConnection();
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
                    UserName TEXT NOT NULL, 
                    GameResults TEXT NOT NULL
                    );
               CREATE TABLE IF NOT EXISTS 
               ActiveGameSessions(
                    GameSessionId TEXT NOT NULL UNIQUE,
                    UserId INT NOT NULL, 
                    UserName TEXT NOT NULL,
                    Questions TEXT NOT NULL,
                    SessionTime TEXT NOT NULL
                );
               CREATE TABLE IF NOT EXISTS 
               DeactivatedGameSessions(
                    GameSessionId TEXT NOT NULL UNIQUE,
                    UserId INT NOT NULL, 
                    UserName TEXT NOT NULL,
                    Questions TEXT NOT NULL,
                    SessionTime TEXT NOT NULL
                );
            ";

            await connection.ExecuteAsync(sql);
        }
    }
}
