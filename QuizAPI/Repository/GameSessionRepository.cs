using Dapper;
using QuizAPI.Dto;
using QuizAPI.Models;
using System;
using System.Data;
using System.Data.SQLite;

namespace QuizAPI.Repository
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public GameSessionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<GameSessionDto> GetGameSession(int userRequestedQuestions = 10)
        {
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
                                                SELECT 
	                                                (SELECT CAST(round(LowerScore / TotalQuestionCount, 1) * 10 AS INT)) AS LowerScorePercentage,
	                                                (SELECT CAST(round(HigherScore / TotalQuestionCount, 1) * 10 AS INT)) AS HigherScorePercentage  From {randomTableName} ;
                                                Drop TABLE {randomTableName};";

                var questionsOfDifferentScoreCount = await connection.QuerySingleOrDefaultAsync<QuestionsCount>(howManyQuestionsToSelect);

                var getLowScoreIdsSql = "Select QuestionId  from Questions where QuestionScore = 5";
                var lowScoreIds = await connection.QueryAsync<int>(getLowScoreIdsSql);
                IEnumerable<int> LowScoreQuestions = GenerateRandomTable(lowScoreIds.ToArray(), questionsOfDifferentScoreCount!.LowerScorePercentage);


                var getHighScoreIdsSql = "Select QuestionId  from Questions where QuestionScore = 10";
                var highScoreIds = await connection.QueryAsync<int>(getHighScoreIdsSql);
                IEnumerable<int> HighScoreQuestions = GenerateRandomTable(highScoreIds.ToArray(), questionsOfDifferentScoreCount!.HigherScorePercentage);

                var questionIdsList = LowScoreQuestions.Concat(HighScoreQuestions).ToArray();

                string finalSql = CreateSelectQuestionsSqlStatement(questionIdsList);
                var questionsList = await connection.QueryAsync<Question>(finalSql);
                questionsList = questionsList.ToList();

                //Change it to the ConstructGameSessionObject method
                return new GameSessionDto();
            }

        }
 
        private static string CreateSelectQuestionsSqlStatement(int[] questionIds)
        {
            string convertedIds = "";
            for (int i = 0; questionIds.Length > i; i++)
            {
                if (i == 0)
                {
                    convertedIds += $"({questionIds[i]},";
                }
                else if (i == questionIds.Length - 1)
                {
                    convertedIds += $"{questionIds[i]})";
                }
                else
                {
                    convertedIds += $"{questionIds[i]},";
                }
            }

            string sql = $"Select * from Questions where QuestionId IN {convertedIds}";
            return sql;
        }
            
        private GameSessionDto ConstructGameSessionObject()
        {
            //To finish up later
            return new GameSessionDto();
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

        private static List<int> GenerateRandomTable(IEnumerable<int> Ids, int numbersToSelect)
        {
            int[] tempArray = Ids.ToArray();
            List<int> randomIds = new List<int>();

            Random random = new();
            for (int i = 1; numbersToSelect >= i; i++)
            {
                int randomIndex = random.Next(0, tempArray.Length);
                randomIds.Add(tempArray[randomIndex]);
                tempArray = tempArray.Where((source, index) => index != randomIndex).ToArray();

            }
            return randomIds;
        }
    }
}
