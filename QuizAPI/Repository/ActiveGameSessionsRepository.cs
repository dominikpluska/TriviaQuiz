﻿using Dapper;
using Microsoft.Extensions.Configuration;
using QuizAPI.Dto;
using QuizAPI.HelperMethods;
using QuizAPI.Models;
using System;
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
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @$"SELECT GameSessionId, UserId, SessionTime 
                         FROM ActiveGameSessions WHERE UserId = {id}";
            var resutls = await connection.QueryFirstOrDefaultAsync<GameSessionDto>(sql);

            return resutls;
        }

        //public async Task<GameSessionDto> GetActiveGameSession(string userName)
        //{
        //    using var connection = SqlConnection.CreateConnection(_connectionString);
        //    var sql = @$"SELECT GameSessionId, UserId, UserName, SessionTime 
        //                 FROM ActiveGameSessions WHERE UserName = '{userName}'";
        //    var resutls = await connection.QueryFirstOrDefaultAsync<GameSessionDto>(sql);

        //    return resutls;
        //}

        public async Task<IEnumerable<string>> GetAllGamesessionGuids()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = $@"Select GameSessionId from ActiveGameSessions";
            var result = await connection.QueryAsync<string>(sql);

            return result;
        }

        public async Task<GameSessionDto> GetActiveGameSessionByGuid(string guid)
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @$"SELECT GameSessionId, UserId, SessionTime 
                         FROM ActiveGameSessions WHERE GameSessionId = '{guid}'";
            var resutls = await connection.QueryFirstOrDefaultAsync<GameSessionDto>(sql);

            return resutls;
        }

        public async Task<int> GetActiveGameSessionCount()
        {
            using var connection = SqlConnection.CreateConnection(_connectionString);
            var sql = @$"SELECT Count(*) FROM ActiveGameSessions";
            var resutls = await connection.ExecuteScalarAsync<int>(sql);

            return resutls;
        }
    }
}
