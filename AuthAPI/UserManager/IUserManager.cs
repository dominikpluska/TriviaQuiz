using AuthAPI.Dto;

namespace AuthAPI.UserManager
{
    public interface IUserManager
    {
        public Task<IResult> RegisterNewUser(UserDto userDto);
        public Task<IResult> Login(UserLoginDto userLoginDto);
        public Task<IResult> CheckAuthentication();
        public IResult Logout();
        public Task<IResult> GetUser(string userName);
        public Task<IResult> GetUserNameAndMail();
        public Task<IResult> ChangeUserNameAndEmail(UserNameAndMailDto userNameAndMailDto);
        public Task<IResult> ChangePassword(ChangePasswordDto changePasswordDto);

    }
}
