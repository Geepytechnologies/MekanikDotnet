﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public Guid ParticipantId { get; set; }
        public ApplicationUser? Participant { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Message> Messages { get; set; } = [];
    }
}
