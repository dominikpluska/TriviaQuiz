using AuthAPI.Commands;
using AuthAPI.CookieGenerator;
using AuthAPI.Dto;
using AuthAPI.JwtGenerator;
using AuthAPI.Models;
using AuthAPI.Repository;
using AuthAPI.UserAccessor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace AuthAPI.UserManager
{
    public class UserManager : IUserManager
    {
        private readonly IAccountsCommands _accountsCommands;
        private readonly IAccountsRepository _accountsRepository;
        private readonly ICreateJwtToken _createToken;
        private readonly ICookieGenerator _cookieGenerator;
        private readonly IUserAccessor _userAccessor;

        public UserManager(IAccountsCommands accountsCommands, IAccountsRepository accountsRepository, ICreateJwtToken createJwtToken,
                            ICookieGenerator cookieGenerator, IUserAccessor userAccessor)
        {
   
            _createToken = createJwtToken;
            _accountsCommands = accountsCommands;
            _accountsRepository = accountsRepository;
            _cookieGenerator = cookieGenerator;
            _userAccessor = userAccessor;
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
                    var cookieUserName = _cookieGenerator.GenerateCookie(DateTime.Now.AddHours(1));
                    var cookie = _cookieGenerator.GenerateCookie(DateTime.Now.AddHours(1));
                    _userAccessor.SetCookie("TriviaQuiz", _createToken.GenerateToken(userAccount.UserName), cookie);
                    _userAccessor.SetCookie("TriviaQuizUserName", userAccount.UserName, cookieUserName);

                    return Results.Ok("Login successful");
                }
            }
        }

        //To be implemented!
        public async Task<IResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userName = _userAccessor.UserName;
            var userToDisplayDto = await _accountsRepository.GetUser(userName);

            var userAccount = await _accountsRepository.GetUser(userName);

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, userAccount!.PasswordHash))
            {
                return Results.Problem("Password is incorrect!");
            }
            else
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                await _accountsCommands.UpdatePassword(passwordHash, userToDisplayDto.UserId);
                return Results.Ok("Password updated");
            }
        }

        public async Task<IResult> CheckAuthentication()
        {
            var token = _userAccessor.TokenString;
            var userName = _userAccessor.UserName;
            var userToDisplayDto = await _accountsRepository.GetUser(userName);

            if (token != null && userToDisplayDto != null)
            {
                return Results.Ok(new {message = "Authenticated", user = userName, isGameMaster = userToDisplayDto.IsGameMaster});
            }
            else
            {
                return Results.Unauthorized();
            }
        }

        public IResult Logout()
        {
            var user = _userAccessor.UserName;
            if (user != null)
            {
                var cookie = _cookieGenerator.GenerateCookie(DateTime.Now.AddDays(-1));
                var cookieUserName = _cookieGenerator.GenerateCookie(DateTime.Now.AddHours(-1));

                _userAccessor.SetCookie("TriviaQuiz", "", cookie);
                _userAccessor.SetCookie("TriviaQuizUserName", "", cookieUserName);
                return Results.Ok(new { message = "Logged out successfully" });
            }
            else
            {
                return Results.BadRequest("No cookie detected!");
            }
        }

        public async Task<IResult> GetUser(string userName)
        {
            var user = await _accountsRepository.GetUser(userName);
            UserToDisplayDto userToDisplayDto = new() {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive,
                IsGameMaster = user.IsGameMaster,
            };
            return Results.Ok(userToDisplayDto);
        }

        public async Task<IResult> GetUserNameAndMail()
        {
            var userName = _userAccessor.UserName;
            var userToDisplayDto = await _accountsRepository.GetUser(userName);

            var userNameAndEmail = await _accountsRepository.GetUserNameAndMail(userToDisplayDto.UserId);

            return Results.Ok(userNameAndEmail);

        }

        public async Task<IResult> ChangeUserNameAndEmail(UserNameAndMailDto userNameAndMailDto)
        {
            var userName = _userAccessor.UserName;
            var userToDisplayDto = await _accountsRepository.GetUser(userName);

            var checkIfUserExist = await _accountsRepository.GetUser(userNameAndMailDto.UserName);
            if (checkIfUserExist != null && checkIfUserExist.UserName != userName)
            {
                return Results.Problem("UserName already exists! Please choose different user name");
            }
            var checkIfEmailIsBound = await _accountsRepository.GetUserEmail(userNameAndMailDto.Email);

            if (checkIfEmailIsBound != null && checkIfEmailIsBound != userToDisplayDto!.Email)
            {
                return Results.Problem("This email is already bound to another account! Please choose different email!");
            }

            var userFromDb = await _accountsRepository.GetUser(userName);
            var result = await _accountsCommands.UpdateUserNameAndEmail(userNameAndMailDto, userFromDb.UserId);

            var cookieUserName = _cookieGenerator.GenerateCookie(DateTime.Now.AddHours(8));
            var cookie = _cookieGenerator.GenerateCookie(DateTime.Now.AddHours(1));

            _userAccessor.SetCookie("TriviaQuiz", _createToken.GenerateToken(userNameAndMailDto.UserName), cookie);
            _userAccessor.SetCookie("TriviaQuizUserName", userNameAndMailDto.UserName, cookieUserName);

            return Results.Ok(result);
        }
    }
}
