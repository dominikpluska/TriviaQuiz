using AuthAPI.Dto;
using AuthAPI.Models;

namespace AuthAPI.Repository
{
    public interface IAccountsRepository
    {
        public Task<IEnumerable<UserToDisplayDto>> GetAll();
        public Task<UserToDisplayDto> GetUser(int id);
        public Task<User> GetUser(string username);
        public Task<string> GetUserEmail(string email);
    }
}
