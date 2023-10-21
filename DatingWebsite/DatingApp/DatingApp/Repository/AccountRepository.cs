using DatingApp.Data;
using DatingApp.IRepository;
using DatingApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext context;

        public AccountRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<AppUser> AddUser(AppUser user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<AppUser> FindUserByUsername(string username)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<bool> IsUserExists(string username)
        {
           return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
