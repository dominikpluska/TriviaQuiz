using AuthAPI.Commands;
using AuthAPI.Dto;
using AuthAPI.JwtGenerator;
using AuthAPI.Models;
using AuthAPI.Repository;
using Microsoft.Extensions.Configuration;

namespace AuthAPI.UserManager
{
    public class UserManager : IUserManager
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountsCommands _accountsCommands;
        private readonly IAccountsRepository _accountsRepository;
        private readonly IJwtCommands _jwtCommands;
        private readonly ICreateJwtToken _createToken;

        public UserManager(IConfiguration configuration, IAccountsCommands accountsCommands, IAccountsRepository accountsRepository, 
               ICreateJwtToken createJwtToken, IJwtCommands jwtCommands)
        {
            _configuration = configuration;
            _createToken = createJwtToken;
            _jwtCommands = jwtCommands;
            _accountsCommands = accountsCommands;
            _accountsRepository = accountsRepository;
        }

        public async Task RegisterNewUser(UserDto userDto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            User user = new();
            user.PasswordHash = passwordHash;
            user.UserName = userDto.UserName;
            user.Email = userDto.Email;

            await _accountsCommands.Insert(user);
        }

        public async Task<IResult> Login(UserLoginDto userLoginDto)
        {
            var userAccount = await _accountsRepository.GetUser(userLoginDto.UserName);

            if (userAccount == null)
            {
                return Results.NotFound("User doesn't exist!");
            }
            else
            {
                if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, userAccount!.PasswordHash))
                {
                    return Results.NotFound("Login or Password were incorrect!");
                }

                else
                {
                    Jwt jwt = new()
                    {
                        UserId = userAccount.UserId,
                        Token = _createToken.GenerateToken(userAccount.UserName)
                    };
                    await _jwtCommands.Insert(jwt);
                    return Results.Ok(jwt);
                }
            }
        }

        //To be implemented!
        public async Task<IResult> ChangePassword(int id, string password)
        {
            var userFromDB = await _accountsRepository.GetUser(id);
            if (userFromDB == null)
            {
                return Results.NotFound();
            }
            else
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                User user = new()
                {
                    UserName = userFromDB.UserName,
                    PasswordHash = passwordHash,

                };
                await _accountsCommands.Update(user);
                return Results.Ok("Password updated");
            }
            
        }
    }
}
