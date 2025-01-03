namespace QuizAPI.Dto
{
    public class CachedGamesDto
    {
        public int Score { get; set; }
        public int AnsweredQuestions { get; set; }
        public int TotalQuestionCount { get; set; }
        public string TimePlayed { get; set; }
    }
}
