using AuthAPI.Commands;
using AuthAPI.Dto;
using AuthAPI.Models;
using AuthAPI.Repository;
using AuthAPI.UserAccessor;
using System.Text.Json;

namespace AuthAPI.AdminManger
{
    public class AdminManager : IAdminManager
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly IAccountsCommands _accountsCommands;
        private readonly IUserAccessor _userAccessor;

        public AdminManager(IAccountsRepository accountsRepository, IAccountsCommands accountsCommands, IUserAccessor userAccessor)
        {
            _accountsRepository = accountsRepository;
            _accountsCommands = accountsCommands;
            _userAccessor = userAccessor;


        }

        public async Task<IResult> GetAllUsers()
        {
            try
            {
                var isGameMaster = await CheckIfUserIsGameMaster();
                if (isGameMaster)
                {
                    var users = await _accountsRepository.GetAllUsers();
                    return Results.Ok(users);
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex) 
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        public async Task<IResult> GetUserById(int id)
        {
            try
            {
                var isGameMaster = await CheckIfUserIsGameMaster();
                if (isGameMaster) 
                {
                    var user = await _accountsRepository.GetUser(id);
                    if (user == null)
                    {
                        return Results.Problem("User does not exist!");
                    }
                    return Results.Ok(user);
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        public async Task<IResult> AddNewUser(UserDto userDto)
        {
            try
            {
                var isGameMaster = await CheckIfUserIsGameMaster();
                if (isGameMaster)
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
                    user.IsGameMaster = userDto.IsGameMaster;
                    user.IsActive = userDto.IsActive;

                    var result = await _accountsCommands.Insert(user);
                    return result;
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        public async Task<IResult> UpdateUser(UserDto user)
        {
            try
            {
                var isGameMaster = await CheckIfUserIsGameMaster();
                if (isGameMaster)
                {
                    var checkIfUserExists = _accountsRepository.GetUser(user.UserId);
                    if (checkIfUserExists != null)
                    {
                        await _accountsCommands.Update(user);
                        return Results.Ok("User has been updated");
                    }
                    else
                    {
                        return Results.Problem("User not found!");
                    }
                }
                else
                {
                    return Results.Unauthorized();
                }

            }
            catch(Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
            
        }

        public async Task<IResult> ChangeUserPassword(int userId,  string password)
        {
            try
            {
                var isGameMaster = await CheckIfUserIsGameMaster();
                if (isGameMaster)
                {
                    var checkIfUserExists = _accountsRepository.GetUser(userId);
                    if (checkIfUserExists != null)
                    {
                        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                        await _accountsCommands.UpdatePassword(passwordHash, userId);
                        return Results.Ok("User has been updated");
                    }
                    else
                    {
                        return Results.Problem("User not found!");
                    }
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
            

        }

        public async Task<IResult> DeleteUser(int id)
        {
            try
            {
                var isGameMaster = await CheckIfUserIsGameMaster();
                if (isGameMaster)
                {
                    var result = await _accountsCommands.Delete(id);
                    return result;
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch(Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        public async Task<IResult> DeactivateUser(int id)
        {
            try
            {
                var isGameMaster = await CheckIfUserIsGameMaster();
                if (isGameMaster)
                {
                    var result = await _accountsCommands.DeactivateUser(id);
                    return result;
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch(Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        private async Task<bool> CheckIfUserIsGameMaster()
        {
            var user = _userAccessor.UserName;
            var userData = await _accountsRepository.GetUser(user);

            if (userData.IsGameMaster == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
