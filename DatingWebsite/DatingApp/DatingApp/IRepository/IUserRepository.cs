﻿using DatingApp.DTOs;
using DatingApp.Models;

namespace DatingApp.IRepository
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUserAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByNameAsync(string username);
        Task<IEnumerable<MemberDto>>GetMembersAsync();
        Task<MemberDto>GetMemberAsync(string username);
    }
}
