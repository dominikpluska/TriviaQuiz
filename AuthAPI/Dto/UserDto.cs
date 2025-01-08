using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Dto
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int IsGameMaster { get; set; } = 0;
        public int IsActive { get; set; } = 1;
    }
}
