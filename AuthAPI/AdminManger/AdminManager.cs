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

        public async Task<IEnumerable<UserToDisplayDto>> GetAllUsers()
        {
           var result =  await _accountsRepository.GetAll();
           return result.ToList();
        }

        public async Task<UserToDisplayDto> GetUserById(int id)
        {
            var result = await _accountsRepository.GetUser(id);
            return result;
        }

        public async Task<IResult> AddNewUser(UserDto userDto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            User user = new();
            user.PasswordHash = passwordHash;
            user.UserName = userDto.UserName;
            user.Email = userDto.Email;

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
