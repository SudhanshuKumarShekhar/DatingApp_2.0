using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
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
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUser()
        {
            // var users = await userRepository.GetUserAsync();
            // var userToReturn = mapper.Map<IEnumerable<MemberDto>>(users);
            //  return Ok(userToReturn);
            var users = await userRepository.GetMembersAsync();
            return Ok(users);
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<AppUser>> getuser(int id)
        //{
        //    return await userRepository.GetUserByIdAsync(id);
        //}
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            // var user = await userRepository.GetUserByNameAsync(username);
            // return mapper.Map<MemberDto>(user);
            if (username == null) NotFound();
            return await userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userRepository.GetUserByNameAsync(username);
            if (user == null) { NotFound(); }

            mapper.Map(memberUpdateDto,user);

            if (await userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
        }
    }
}
