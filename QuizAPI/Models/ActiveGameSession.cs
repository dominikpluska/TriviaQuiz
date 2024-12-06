using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models
{
    public class ActiveGameSession
    {
        [Required]
        public string GameSessionId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Questions { get; set; }
        public string SessionTime { get; set; } = DateTime.Now.ToString();
    }
}
