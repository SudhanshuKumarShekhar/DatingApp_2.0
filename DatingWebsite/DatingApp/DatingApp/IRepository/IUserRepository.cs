using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;

namespace DatingApp.IRepository
{
    public interface IUserRepository
    {
        void Update(AppUser user);
       
        Task<IEnumerable<AppUser>> GetUserAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByNameAsync(string username);
        Task<PagedList<MemberDto>>GetMembersAsync(UserParams userParams);
        Task<MemberDto>GetMemberAsync(string username);
    }
}
