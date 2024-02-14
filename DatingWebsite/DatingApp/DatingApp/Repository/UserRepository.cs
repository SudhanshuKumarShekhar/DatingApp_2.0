using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.IRepository;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository( DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
           // var query = context.Users
            //   .ProjectTo<MemberDto>(mapper.ConfigurationProvider).AsNoTracking();
           // return await PagedList<MemberDto>.CreateAsync(
            //      query,
             //    userParams.PageNumber,
             //     userParams.PageSize);


            // return await context.Users
            //    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            //     .ToListAsync();

            var query = context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDto>(mapper.ConfigurationProvider),
               userParams.PageNumber,
                userParams.PageSize);

        }

        public async Task<IEnumerable<AppUser>> GetUserAsync()
        {
            return await context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByNameAsync(string username)
        {
            return await context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        }

       

        public void Update(AppUser user)
        {
            context.Entry(user).State = EntityState.Modified;
        }
    }
}
