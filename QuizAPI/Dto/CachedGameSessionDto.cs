using QuizAPI.Models;

namespace QuizAPI.Dto
{
    public class CachedGameSessionDto
    {
        public string GameSessionId { get; set; }
        public IEnumerable<QuestionsCaching> Questions { get; set; }
        public int Score { get; set; }
        public int AnsweredQuestions { get; set; }
        public int TotalQuestionCount { get; set; }
        public string SessionTime { get; set; }
    }
}
