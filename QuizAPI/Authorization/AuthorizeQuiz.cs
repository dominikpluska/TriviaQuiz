using QuizAPI.Dto;
using QuizAPI.Services;
using QuizAPI.UserAccessor;
using System.Text.Json;

namespace QuizAPI.Authorization
{
    public class AuthorizeQuiz : IAuthorizeQuiz
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IAuthenticationService _authenticationService;
        public AuthorizeQuiz(IUserAccessor userAccessor, IAuthenticationService authenticationService)
        {
            _userAccessor = userAccessor;
            _authenticationService = authenticationService;
        }

        public async Task<UserToDisplayDto> GetUser()
        {
            var user = _userAccessor.UserName;
            var userData = await _authenticationService.GetUser(user);
            UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;
            return userToDisplayDto;
        }

        public async Task<bool> IsAuthorized()
        {
            var userToDisplayDto = await GetUser();
            if (userToDisplayDto == null)
            {
                return false;
            }
            else if(userToDisplayDto.isActive == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> IsGameMaster()
        {
            var userToDisplayDto = await GetUser();
            if (userToDisplayDto == null)
            {
                return false;
            }
            else if (userToDisplayDto.isActive == 1 && userToDisplayDto.isGameMaster == 1)
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
