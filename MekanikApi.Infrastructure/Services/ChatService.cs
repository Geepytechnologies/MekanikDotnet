using MekanikApi.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Services
{
    public class ChatService: Hub
    {
        private readonly IConversationService _conversationService;
        public ChatService(IConversationService conversationService)
        {
            _conversationService = conversationService; 
        }
        public async Task SendMessage(Guid userId, Guid participantId, string content)
        {
            //var userId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(userId.ToString()))
            {
                throw new HubException("User is not authenticated.");
            }
            var conversation = await _conversationService.GetOrCreateConversationAsync(userId, participantId);
            var message = new Message { ConversationId = conversation.Id, UserId = userId, Content = content };
            

            await _conversationService.SaveMessageAsync(message);

            await Clients.User(participantId.ToString()).SendAsync("ReceiveMessage", message);
        }
    }
}
