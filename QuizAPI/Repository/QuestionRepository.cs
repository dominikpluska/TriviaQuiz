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

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        //public async Task RequestGameSession(int userRequestedQuestions = 10)
        //{
        //    //Check if there is already an active game session. To be implemented later on.

        //    using var connection = CreateConnection();

        //    var sqlCheckCount = "Select count(*) from Questions";
        //    int questionsCount = await connection.ExecuteScalarAsync<int>(sqlCheckCount);

        //    if (questionsCount < 10)
        //    {
        //        throw new ApplicationException("The questions count was less than 10! The database must include at least 10 questions!");
        //    }
        //    else
        //    {
        //        string randomTableName = GenerateRandomTableName();

        //        var howManyQuestionsToSelect = $@"Drop TABLE IF EXISTS {randomTableName};
        //                                        Create Temp Table {randomTableName} AS
        //                                        SELECT   
	       //                                          (SELECT CAST(count(*) AS REAL) FROM Questions) AS TotalQuestionCount,
        //                                             (SELECT CAST(count(*) AS REAL) FROM Questions WHERE QuestionScore = 5) AS LowerScore,   
        //                                             (SELECT CAST(count(*) AS REAL) FROM Questions WHERE QuestionScore = 10) AS HigherScore;
        //                                        Select 
	       //                                         (Select Cast(round(LowerScore / TotalQuestionCount, 1) * 10 as INT)) as LowerScorePercentage,
	       //                                         (Select Cast(round(HigherScore / TotalQuestionCount, 1) * 10 as INT)) as HigherScorePercentage  From {randomTableName} ;
        //                                        Drop TABLE {randomTableName};";

        //        var questionsOfDifferentScoreCount = await connection.QuerySingleOrDefaultAsync<QuestionsCount>(howManyQuestionsToSelect);

        //        var getLowScoreIdsSql = "Select QuestionId  from Questions where QuestionScore = 5";
        //        var lowScoreIds = await connection.QueryAsync<int>(getLowScoreIdsSql);
        //        IEnumerable<int> LowScoreQuestions = GenerateRandomTable(lowScoreIds.ToArray(), questionsOfDifferentScoreCount!.LowerScorePercentage);


        //        var getHighScoreIdsSql = "Select QuestionId  from Questions where QuestionScore = 10";
        //        var highScoreIds = await connection.QueryAsync<int>(getHighScoreIdsSql);
        //        IEnumerable<int> HighScoreQuestions = GenerateRandomTable(highScoreIds.ToArray(), questionsOfDifferentScoreCount!.HigherScorePercentage);

        //        var questionsList = LowScoreQuestions.Concat(HighScoreQuestions).ToList();

        //        int a = 1;

        //    }
        //}


        //private static string GenerateRandomTableName()
        //{
        //    Random random = new();
        //    string alphabet = "abcdefghijklmnopqrstuvwxyz";
        //    string randomTableName = "";

        //    for (int i = 0; i < 21; i++)
        //    {
        //        int x = random.Next(26);
        //        randomTableName = randomTableName + alphabet[x];
        //    }
        //    return randomTableName;
        //}

        //private static List<int> GenerateRandomTable(IEnumerable<int> Ids, int numbersToSelect) 
        //{
        //    int[] tempArray = Ids.ToArray();
        //    List<int> randomIds = new List<int>();

        //    Random random = new();
        //    for (int i = 1; numbersToSelect >= i; i++)
        //    {
        //        int randomIndex = random.Next(0 , tempArray.Length);
        //        randomIds.Add(tempArray[randomIndex]);
        //        tempArray = tempArray.Where((source, index) => index != randomIndex).ToArray();

        //    }
        //    return randomIds;
        //}
    }
}
