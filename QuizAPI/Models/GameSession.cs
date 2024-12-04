using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace QuizAPI.Models
{
    public class GameSession
    {
        [Key]
        [Required]
        public string GameSessionId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        List<Question> questions { get; set; }

    }
}
