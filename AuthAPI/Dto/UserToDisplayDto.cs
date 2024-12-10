using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Dto
{
    public class UserToDisplayDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int IsGameMaster { get; set; }
        public int IsActive { get; set; } 
    }
}
