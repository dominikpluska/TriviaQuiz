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
        private readonly IAccountsCommands _accountsCommands;
        private readonly IAccountsRepository _accountsRepository;
        private readonly IJwtCommands _jwtCommands;
        private readonly ICreateJwtToken _createToken;

        public UserManager(IAccountsCommands accountsCommands, IAccountsRepository accountsRepository, ICreateJwtToken createJwtToken,
                            IJwtCommands jwtCommands)
        {
   
            _createToken = createJwtToken;
            _jwtCommands = jwtCommands;
            _accountsCommands = accountsCommands;
            _accountsRepository = accountsRepository;
        }

        public async Task<IResult> RegisterNewUser(UserDto userDto)
        {
            var checkIfUserExist = await _accountsRepository.GetUser(userDto.UserName);
            if (checkIfUserExist != null)
            {
                return Results.Problem("UserName already exists!");
            }
            var checkIfEmailIsBound = await _accountsRepository.GetUserEmail(userDto.Email);

            if (checkIfEmailIsBound != null)
            {
                return Results.Problem("This email is already bound to another account! Please contanct support for help!!");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            User user = new();
            user.PasswordHash = passwordHash;
            user.UserName = userDto.UserName;
            user.Email = userDto.Email;

            await _accountsCommands.Insert(user);
            return Results.Ok("Account has been created!");
        }

        public async Task<IResult> Login(UserLoginDto userLoginDto)
        {
            var userAccount = await _accountsRepository.GetUser(userLoginDto.UserName);


            if (userAccount == null)
            {
                return Results.NotFound("Login or Password were incorrect!");
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

                    JwtDto jwtDto = new()
                    {
                        UserId = userAccount.UserId,
                        UserName = userAccount.UserName,
                        Token = jwt.Token,
                    };

                    return Results.Ok(jwtDto);
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
