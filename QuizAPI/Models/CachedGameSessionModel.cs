using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models
{
    public class CachedGameSessionModel
    {
        public string GameSessionId { get; set; } 
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string questions { get; set; }
        public string SessionTime { get; set; } 
    }
}
