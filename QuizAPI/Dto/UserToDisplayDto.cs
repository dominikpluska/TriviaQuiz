namespace QuizAPI.Dto
{
    public class UserToDisplayDto
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public int isGameMaster { get; set; }
        public int isActive { get; set; }
    }
}
