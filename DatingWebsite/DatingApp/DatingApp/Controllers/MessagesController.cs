using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.IRepository;
using DatingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
   
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public MessagesController(IUnitOfWork uow,
            IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();
            if(username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You can't send message to yourself.");
            }
            var Sender = await uow.UserRepository.GetUserByNameAsync(username);
            var recipient = await uow.UserRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);
            
            if(recipient == null) { return NotFound(); }

            var message = new Message
            {
                SenderId = Sender.Id,
                SenderUsername = Sender.UserName,
                Sender = Sender,
                Recipient = recipient,
                RecipientId = recipient.Id,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            uow.MessageRepository.AddMessage(message);
            if(await uow.Complete())
            {
                return Ok(mapper.Map<MessageDto>(message));
            }
            return BadRequest("Failed to sent message.");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await uow.MessageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages));

            return messages;

        }

       

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await uow.MessageRepository.GetMessage(id);
            if(message.SenderUsername != username && message.RecipientUsername!=username)
            { return Unauthorized(); }

            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted)
            {
                uow.MessageRepository.DeleteMessage(message);
            }

            if(await uow.Complete()) { return Ok(); }
            return BadRequest("problem deleting the message");
        }
    }
}
