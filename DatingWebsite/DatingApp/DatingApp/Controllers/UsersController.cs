using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.IRepository;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DatingApp.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.photoService = photoService;
        }
        // [AllowAnonymous]
       
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.GetUsername();
            var currentUser = await uow.UserRepository.GetUserByNameAsync(username);
            userParams.CurrentUsername = currentUser.UserName;
            if (string.IsNullOrEmpty(userParams.Gender))
             {
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }
            var users = await uow.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount,
                users.TotalPages));
            return Ok(users);

        }
        /*
         * [HttpGet]
         public async Task<ActionResult<IEnumerable<MemberDto>>> GetUser()
         {
             // var users = await uow.UserRepository.GetUserAsync();
             // var userToReturn = mapper.Map<IEnumerable<MemberDto>>(users);
             //  return Ok(userToReturn);
             var users = await uow.UserRepository.GetUserAsync();
             return Ok(users);
         }
        */

        //[HttpGet("{id}")]
        //public async Task<ActionResult<AppUser>> getuser(int id)
        //{
        //    return await uow.UserRepository.GetUserByIdAsync(id);
        //}

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            // var user = await uow.UserRepository.GetUserByNameAsync(username);
            // return mapper.Map<MemberDto>(user);
            if (username == null) NotFound();
            return await uow.UserRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.GetUsername();
            var user = await uow.UserRepository.GetUserByNameAsync(username);
            if (user == null) { NotFound(); }

            mapper.Map(memberUpdateDto,user);

            if (await uow.Complete()) return NoContent();
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await uow.UserRepository.GetUserByNameAsync(User.GetUsername());
            if (user == null) { NotFound(); }

            var result = await photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await uow.Complete())
            {
                return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await uow.UserRepository.GetUserByNameAsync(User.GetUsername());
            if (user == null) { NotFound(); }
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo.Id == null) { NotFound(); }
            if ((bool)photo.IsMain) return BadRequest("This is already main photo.");

            var currentMain = user.Photos.FirstOrDefault(x => (bool)x.IsMain);
            if(currentMain != null) { currentMain.IsMain = false; }
            photo.IsMain = true;

            if(await uow.Complete()) { return NoContent(); }

            return BadRequest("Problem setting the main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto (int photoId)
        {
            var user = await uow.UserRepository.GetUserByNameAsync(User.GetUsername());
            if (user == null) { NotFound(); }
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo.Id == null) { NotFound(); }
            if ((bool)photo.IsMain) return BadRequest("you cannot delete your main photo.");
            if(photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await uow.Complete()) { return Ok(); }

            return BadRequest("Problem deleting photo");
        }

    }
}
