﻿using Dapper;
using Microsoft.Extensions.Configuration;
using QuizAPI.Dto;
using QuizAPI.Models;
using System.Data.SQLite;

namespace QuizAPI.Repository
{
    public class ActiveGameSessionsRepository : IActiveGameSessionsRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public ActiveGameSessionsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<GameSessionDto> GetActiveGameSession(int id)
        {
            using var connection = CreateConnection();
            var sql = @$"SELECT GameSessionId, UserId, UserName, SessionTime 
                         FROM ActiveGameSessions WHERE UserId = {id}";
            var resutls = await connection.QueryFirstOrDefaultAsync<GameSessionDto>(sql);

            return resutls;
        }

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}