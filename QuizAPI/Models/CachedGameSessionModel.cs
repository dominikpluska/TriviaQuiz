using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models
{
    public class CachedGameSessionModel
    {
        public string GameSessionId { get; set; } 
        public int UserId { get; set; }
        public string Questions { get; set; }
        public int Score { get; set; }
        public int AnsweredQuestions { get; set; }
        public int TotalQuestionCount { get; set; }
        public string SessionTime { get; set; } 
    }
}
