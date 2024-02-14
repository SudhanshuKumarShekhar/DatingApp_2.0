using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.IRepository;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository
{
    public class LikeRepository : ILikesRepository
    {
        private readonly DataContext context;

        public LikeRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParam likesParam)
        {
            var users = context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = context.Likes.AsQueryable();

            if(likesParam.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParam.UserId);
                users = likes.Select(like => like.TragetUser);
            }
            if (likesParam.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TragetUserId == likesParam.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUser = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUser, likesParam.PageNumber, likesParam.PageSize);
        }

        public async Task<AppUser> GetUserwithLikes(int UserId)
        {
            return await context.Users.Include(x => x.LikedUsers)
                 .FirstOrDefaultAsync(x => x.Id == UserId);
        }

       
    }
}
