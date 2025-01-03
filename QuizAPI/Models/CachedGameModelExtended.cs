namespace QuizAPI.Models
{
    public class CachedGameModelExtended
    {
        public string GameSessionId { get; set; }
        public int Score { get; set; }
        public int TotalQuestionCount { get; set; }
        public int AnsweredQuestions { get; set; }
        public string SessionTime { get; set; }
    }
}
