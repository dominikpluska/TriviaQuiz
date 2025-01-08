using AuthAPI.Commands;
using AuthAPI.Dto;
using AuthAPI.Models;
using AuthAPI.Repository;

namespace AuthAPI.AdminManger
{
    public class AdminManager : IAdminManager
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly IAccountsCommands _accountsCommands;

        public AdminManager(IAccountsRepository accountsRepository, IAccountsCommands accountsCommands)
        {
            _accountsRepository = accountsRepository;
            _accountsCommands = accountsCommands;
            
        }

        public async Task<IResult> GetAllUsers()
        {
           var users =  await _accountsRepository.GetAllUsers();
            return Results.Ok(users);
        }

        public async Task<IResult> GetUserById(int id)
        {
            var user = await _accountsRepository.GetUser(id);
            if(user == null)
            {
                return Results.Problem("User does not exist!");
            }
            return Results.Ok(user);
        }

        public async Task<IResult> AddNewUser(UserDto userDto)
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

        public async Task<IResult> UpdateUser(User user)
        {
            var result = await _accountsCommands.Update(user);
            return result;
        }

        public async Task<IResult> DeleteUser(int id)
        {
            var result = await _accountsCommands.Delete(id);
            return result;
        }

        public async Task<IResult> DeactivateUser(int id)
        {
            var result = await _accountsCommands.DeactivateUser(id);
            return result;
        }

    }
}
