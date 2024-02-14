using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;

namespace DatingApp.IRepository
{
    public interface ILikesRepository
    {
      
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        Task<AppUser> GetUserwithLikes(int UserId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParam likesParam);
    }
}
