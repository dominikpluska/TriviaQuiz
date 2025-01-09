using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizAPI.Dto
{
    public class QuestionExtendedDto
    {
        public required string QuestionTitle { get; set; }
        public required string QuestionDescription { get; set; }
        public required string QuestionCategory { get; set; }
        public required string OptionA { get; set; }
        public required string OptionB { get; set; }
        public required string OptionC { get; set; }
        public required string OptionD { get; set; }
        public required string CorrectAnswer { get; set; }
        public required int QuestionScore { get; set; }
    }
}
