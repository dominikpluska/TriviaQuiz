using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizAPI.Dto
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionDescription { get; set; }
        public string QuestionCategory { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public int QuestionScore { get; set; }
    }
}
