using AuthAPI.Dto;
using AuthAPI.HelperMethods;
using AuthAPI.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace AuthAPI.Commands 
{ 
    public class AccountsCommands : IAccountsCommands
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        public AccountsCommands(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<IResult> Insert(User user)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"INSERT INTO Accounts (UserName, Email, PasswordHash, IsGameMaster, IsActive) 
                        VALUES (@UserName, @Email, @PasswordHash, @IsGameMaster, @IsActive)";
            await connection.ExecuteAsync(sql, user);

            return Results.Ok("A new record has been created!");
        }

        public async Task<IResult> Update(User user)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"UPDATE Accounts SET UserName = @UserName, Email = @Email, PasswordHash = @PasswordHash, 
                        IsGameMaster = @IsGameMaster, IsActive = @IsActive where UserId = {user.UserId}";
            await connection.ExecuteAsync(sql, user);

            return Results.Ok($"{user.UserId} has been updated");
        }
        public async Task<IResult> Delete(int id)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"Delete FROM Accounts WHERE UserId = {id}";
            await connection.ExecuteAsync(sql);

            return Results.Ok($"{id} has been deleted");
        }
        public async Task<IResult> DeactivateUser(int id)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"UPDATE Accounts SET IsActive = 0 WHERE UserId = {id}";
            await connection.ExecuteAsync(sql);

            return Results.Ok($"{id} has been updated");
        }
    }
}
