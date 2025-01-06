using AuthAPI.Dto;
using AuthAPI.Models;

namespace AuthAPI.Commands
{
    public interface IAccountsCommands
    {
        public Task<IResult> Insert(User user);
        public Task<IResult> Update(User user);
        public Task<IResult> Delete(int id);
        public Task<IResult> DeactivateUser(int id);
        public Task<UserNameAndMailDto> UpdateUserNameAndEmail(UserNameAndMailDto userNameAndMailDto, int userId);
        public Task<IResult> UpdatePassword(string password, int userId);
    }
}
