using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models
{
    public class QuizResults
    {
        [Key]
        [Required]
        public int ResultId { get; set; }
        [Required]
        public required string GameSessionId { get; set; } 
        [Required]
        public required string GameResults { get; set; }
    }
}
