using Microsoft.Extensions.Configuration;

namespace AuthAPI.UserManager
{
    public class UserManager
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public UserManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task RegisterNewUser()
        {

        }

        public async Task Login()
        {

        }
    }
}
