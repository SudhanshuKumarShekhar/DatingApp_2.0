﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.IRepository;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await context.Groups
                 .Include(x => x.Connections)
                 .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                 .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var quary = context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();
            quary = messageParams.Container switch
            {
                "Inbox" => quary.Where(u =>u.RecipientUsername == messageParams.Username && u.RecipientDeleted==false),
                "Outbox" => quary.Where(u =>u.SenderUsername == messageParams.Username && u.SenderDeleted==false),
                _ => quary.Where(u =>u.RecipientUsername == messageParams.Username && u.RecipientDeleted==false && u.DateRead ==null)
            };

            var messages = quary.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await context.Groups
                .Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var query = context.Messages
                .Where(m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
                m.SenderUsername == recipientUserName ||
                m.RecipientUsername == recipientUserName && m.SenderDeleted == false &&
                m.SenderUsername == currentUserName).OrderBy(m => m.MessageSent)
                .AsQueryable();

            var unreadMessage = query.Where(m => m.DateRead == null && m.RecipientUsername == currentUserName)
                                        .ToList();
            if (unreadMessage.Any())
            {
                foreach(var message in unreadMessage)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                
            }
            return await query.ProjectTo<MessageDto>(mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveConnection(Connection connection)
        {
            context.Connections.Remove(connection);
        }

       
    }
}
