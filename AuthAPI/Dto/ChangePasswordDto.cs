namespace AuthAPI.Dto
{
    public class ChangePasswordDto
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string NewPasswordConfirm { get; set; }
    }
}
