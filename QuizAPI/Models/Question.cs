using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAPI.Models
{
    public class Question
    {
        [Key]
        [NotNull]
        public int QuestionId { get; set; }
        [Required]
        [MaxLength(250)]
        [NotNull]
        public required string QuestionTitle { get; set; }
        [Required]
        [NotNull]
        public required string QuestionDescription { get; set; }
        [Required]
        [NotNull]
        public required string QuestionCategory { get; set; }
        [Required]
        [NotNull]
        public required string OptionA { get; set; }
        [Required]
        [NotNull]
        public required string OptionB { get; set; }
        [Required]
        [NotNull]
        public required string OptionC { get; set; }
        [Required]
        [NotNull]
        public required string OptionD { get; set; }
        [Required]
        [NotNull]
        public required string CorrectAnswer { get; set; }
        [Required]
        [NotNull]
        public required int QuestionScore { get; set; }
    }
}
