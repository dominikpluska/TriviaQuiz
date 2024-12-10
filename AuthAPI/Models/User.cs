using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public int IsGameMaster { get; set; } = 0;

        [Required]
        public int IsActive { get; set; } = 1;

    }
}
