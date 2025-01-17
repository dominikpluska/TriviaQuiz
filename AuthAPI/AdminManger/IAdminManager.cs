﻿using AuthAPI.Dto;
using AuthAPI.Models;

namespace AuthAPI.AdminManger
{
    public interface IAdminManager
    {
        public Task<IResult> GetAllUsers();
        public Task<IResult> GetUserById(int id);
        public Task<IResult> AddNewUser(UserDto userDto);
        public Task<IResult> UpdateUser(UserDto user);
        public Task<IResult> ChangeUserPassword(int userId, string password);
        public Task<IResult> DeleteUser(int id);
        public Task<IResult> DeactivateUser(int id);
    }
}
