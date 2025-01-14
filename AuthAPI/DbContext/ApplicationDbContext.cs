﻿using AuthAPI.Dto;
using AuthAPI.HelperMethods;
using AuthAPI.Models;
using Dapper;
using System.Data.SQLite;

namespace AuthAPI.DbContext
{
    public class ApplicationDbContext
    {
        private readonly string _connectionString;
        private readonly string _defaultAdminPassword;
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
            _defaultAdminPassword = _configuration.GetValue<string>("DefaultAdminPassword")!;
        }

        public async Task CreateModel()
        {
            if (!File.Exists($"{Environment.CurrentDirectory}\\Users.db"))
            {
                SQLiteConnection.CreateFile($"{Environment.CurrentDirectory}\\Users.db");
                await CreateTables();
                await SeedTables();
            }
        }

        private async Task CreateTables()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @$"
                        CREATE TABLE IF NOT EXISTS 
                        Accounts (
                            UserId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            UserName TEXT NOT NULL UNIQUE,
                            Email TEXT NOT NULL UNIQUE,
                            PasswordHash TEXT NOT NULL,
                            IsGameMaster INT NOT NULL,
                            IsActive INT NOT NULL
                        );";
            await connection.ExecuteAsync(sql);
        }

        private async Task SeedTables()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"INSERT INTO Accounts(
                        UserName, Email, PasswordHash, IsGameMaster, IsActive)
                        VALUES (@UserName, @Email, @PasswordHash, @IsGameMaster, @IsActive)
                        
            ";

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(_defaultAdminPassword);
            User user = new() {
                UserName = "GameMaster", Email = "GameMaster@gamemaster.com", PasswordHash= passwordHash, IsActive=1, IsGameMaster=1
            };

            await connection.ExecuteAsync(sql, user);
        }
    }
}
