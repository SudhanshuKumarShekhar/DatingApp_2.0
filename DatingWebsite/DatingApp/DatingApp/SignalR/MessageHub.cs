﻿using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using DatingApp.IRepository;
using DatingApp.Models;
using DatingApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    [Authorize]
    public class MessageHub :Hub
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IHubContext<PresenceHub> presenceHub;

        public MessageHub(IUnitOfWork uow, 
            IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await uow.MessageRepository
                .GetMessageThread(Context.User.GetUsername(), otherUser);
            if (uow.HasChanges()) await uow.Complete(); 
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();
            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                throw new HubException("You can't send message to yourself.");
            }
            var sender = await uow.UserRepository.GetUserByNameAsync(username);
            var recipient = await uow.UserRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) { throw new HubException("NotFound user"); }

            var message = new Message
            {
                SenderId = sender.Id,
                SenderUsername = sender.UserName,
                Sender = sender,
                Recipient = recipient,
                RecipientId = recipient.Id,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await uow.MessageRepository.GetMessageGroup(groupName); 
            if(group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if(connections != null)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }
            uow.MessageRepository.AddMessage(message);
            if (await uow.Complete())
            {
                
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
                
            }
        }
        private string GetGroupName (string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) <0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";

        }
        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await uow.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if(group == null)
            {
                group = new Group(groupName);
                uow.MessageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
             if(await uow.Complete()) return group;

            throw new HubException("Failed to Add to group");
        }
        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await uow.MessageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            uow.MessageRepository.RemoveConnection(connection);
            if(await uow.Complete()) return group;

            throw new HubException("Failed to remove from group");
        }
    }
}
