using DatingApp.Models;

namespace DatingApp.IRepository
{
    public interface IAccountRepository
    {
        Task<AppUser> AddUser(AppUser user);
        Task<AppUser> FindUserByUsername(string username);
        Task<bool> IsUserExists(string username);
        Task<bool> SaveAllAsync();
    }
}
