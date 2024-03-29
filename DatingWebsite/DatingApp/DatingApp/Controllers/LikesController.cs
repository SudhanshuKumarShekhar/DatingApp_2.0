﻿using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.IRepository;
using DatingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
   
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork uow;

        public LikesController(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike( string username)
        {
            var sourceUserId = int.Parse(User.GetUserId());
            var likedUser = await uow.UserRepository.GetUserByNameAsync(username);
            var sourceUser = await uow.likesRepository.GetUserwithLikes(sourceUserId);

            if(likedUser == null) { return NotFound(); }

            if(sourceUser.UserName == username) { return BadRequest("You can't Like Yourself"); }

            var userLike = await uow.likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if(userLike != null) { return BadRequest(" you already like this user"); }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TragetUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if(await uow.Complete()) { return Ok(); }

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>>GetUserLikes([FromQuery]LikesParam likesParam)
        {
            likesParam.UserId = int.Parse(User.GetUserId());
            var users = await uow.likesRepository.GetUserLikes(likesParam);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }
    }
}
