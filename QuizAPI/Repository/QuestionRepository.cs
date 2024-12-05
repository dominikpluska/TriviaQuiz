using Dapper;
using QuizAPI.Dto;
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
            using var connection = CreateConnection();

            var sql = @"SELECT * FROM Questions";

            var resutls = await connection.QueryAsync<Question>(sql);

            return resutls.ToList();
        }

        public async Task<Question> GetQuestion(int id)
        {
            using var connection = CreateConnection();
            var sql = @$"SELECT * FROM Questions WHERE QuestionId = {id}";
            var resutls = await connection.QuerySingleAsync<Question>(sql);

            return resutls;
        }

        public async Task RequestGameSession(int userRequestedQuestions = 10)
        {
            //Check if there is already an active game session. To be implemented later on.

            using var connection = CreateConnection();

            var sqlCheckCount = "Select count(*) from Questions";
            int questionsCount = await connection.ExecuteScalarAsync<int>(sqlCheckCount);

            if (questionsCount < 10)
            {
                throw new ApplicationException("The questions count was less than 10! The database must include at least 10 questions!");
            }
            else
            {
                string randomTableName = GenerateRandomTableName();

                var howManyQuestionsToSelect = $@"Drop TABLE IF EXISTS {randomTableName};
                                                Create Temp Table {randomTableName} AS
                                                SELECT   
	                                                 (SELECT CAST(count(*) AS REAL) FROM Questions) AS TotalQuestionCount,
                                                     (SELECT CAST(count(*) AS REAL) FROM Questions WHERE QuestionScore = 5) AS LowerScore,   
                                                     (SELECT CAST(count(*) AS REAL) FROM Questions WHERE QuestionScore = 10) AS HigherScore;
                                                Select 
	                                                (Select Cast(round(LowerScore / TotalQuestionCount, 1) * 10 as INT)) as LowerScorePercentage,
	                                                (Select Cast(round(HigherScore / TotalQuestionCount, 1) * 10 as INT)) as HigherScorePercentage  From {randomTableName} ;
                                                Drop TABLE {randomTableName};";

                var questionsOfDifferentScoreCount = await connection.QuerySingleOrDefaultAsync<QuestionsCount>(howManyQuestionsToSelect);

                var getLowScoreIdsSql = "Select QuestionId  from Questions where QuestionScore = 5";

                var LowScoreIds = await connection.QueryAsync<int>(getLowScoreIdsSql);
                int a = 1;

            }
        }

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        private static string GenerateRandomTableName()
        {
            Random random = new();
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            string randomTableName = "";

            for (int i = 0; i < 21; i++)
            {
                int x = random.Next(26);
                randomTableName = randomTableName + alphabet[x];
            }
            return randomTableName;
        }

        private static IEnumerable<int> GenerateRandomTable(IEnumerable<int> Ids, int numberToSelect) 
        {
            //Select a random question based on its Id. Id must not repeat itself. Number of questions must be equal numberToSelect argument 
            return [];
        }
    }
}
