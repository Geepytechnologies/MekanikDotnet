using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Services
{
    public interface IConversationService
    {
        Task<Conversation> CreateConversationAsync(Guid userId, Guid participantId);
        Task<ICollection<Conversation>> GetConversationsAsync(Guid userId);
        Task<Message> SendMessageAsync(Guid conversationId, Guid userId, string content);
        Task<ICollection<Message>> GetMessagesAsync(Guid conversationId);
    }

    public class ConversationService : IConversationService
    {
        private readonly ApplicationDbContext _dbContext;

        public ConversationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Conversation> CreateConversationAsync(Guid userId, Guid participantId)
        {
            var conversation = new Conversation { UserId = userId, ParticipantId = participantId };
            _dbContext.Conversations.Add(conversation);
            await _dbContext.SaveChangesAsync();
            return conversation;
        }

        public async Task<ICollection<Conversation>> GetConversationsAsync(Guid userId)
        {
            return await _dbContext.Conversations
                .Include(c => c.User)
                .Include(c => c.Participant)
                .Where(c => c.UserId == userId || c.ParticipantId == userId)
                .ToListAsync();
        }

        public async Task<Message> SendMessageAsync(Guid conversationId, Guid userId, string content)
        {
            var message = new Message { ConversationId = conversationId, UserId = userId, Content = content };
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
            return message;
        }

        public async Task<ICollection<Message>> GetMessagesAsync(Guid conversationId)
        {
            return await _dbContext.Messages
                .Include(m => m.User)
                .Where(m => m.ConversationId == conversationId)
                .ToListAsync();
        }
    }
}
