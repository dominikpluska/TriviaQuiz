namespace QuizAPI.Dto
{
    public class QuestionLightModelDto
    {
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionCategory { get; set; }
        public int QuestionScore { get; set; }
        public string CorrectAnswer { get; set; }
        
    }
}
