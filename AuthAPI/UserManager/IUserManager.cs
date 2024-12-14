using AuthAPI.Dto;

namespace AuthAPI.UserManager
{
    public interface IUserManager
    {
        public Task<IResult> RegisterNewUser(UserDto userDto);

        public Task<IResult> Login(UserLoginDto userLoginDto);
    }
}
