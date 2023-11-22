using AutoMapper;
using Azure.Core;
using DatingApp.DTOs;
using DatingApp.Interfaces;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user = mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();

            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            
            var roleResult = await userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token =await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x =>(bool) x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (user == null) return Unauthorized();
            var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!checkPasswordResult)
            {
                return Unauthorized("Invalid Password");
            }
                return new UserDto
            {
                Username = user.UserName,
                Token =await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => (bool)x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
