using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public enum AttachmentType
    {
        None = 0, 
        Image = 1,
        Video = 2,
        File = 3
    }
    public class Message
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public AttachmentType AttachmentType { get; set; } = AttachmentType.None;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
