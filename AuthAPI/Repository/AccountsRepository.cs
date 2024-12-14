using AuthAPI.Dto;
using AuthAPI.HelperMethods;
using AuthAPI.Models;
using Dapper;

namespace AuthAPI.Repository
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        public AccountsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<IEnumerable<UserToDisplayDto>> GetAll()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"SELECT UserId, UserName, Email, IsGameMaster, IsActive 
                        FROM Accounts";

            return await connection.QueryAsync<UserToDisplayDto>(sql);
        }

        public async Task<UserToDisplayDto> GetUser(int id)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"SELECT UserId, UserName, Email, IsGameMaster, IsActive 
                        FROM Accounts Where UserId = {id}";

            return await connection.QuerySingleAsync<UserToDisplayDto>(sql);
        }

        public async Task<User> GetUser(string username)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"SELECT UserId, UserName, Email, IsGameMaster, IsActive, PasswordHash 
                        FROM Accounts Where UserName = '{username}'";

            var result = await connection.QueryAsync<User>(sql);

            return result.FirstOrDefault();
        }

        public async Task<string> GetUserEmail(string email)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"SELECT Email FROM Accounts Where Email = '{email}'";

            var result = await connection.QueryAsync<string>(sql);

            return result.FirstOrDefault();
        }

    }
}
