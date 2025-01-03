namespace QuizAPI.Dto
{
    public class LastPlayedQuizResultDto
    {
        public int Score { get; set; }
        public int AnsweredQuestions { get; set; }
        public int TotalQuestionCount { get; set; }
    }
}
