using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class Feedback
    {
        public Guid Id { get; set; }

        public string? Message { get; set; }

        public Guid FromUserId { get; set; }

        public ApplicationUser? FromUser { get; set; }

        public Guid ToUserId { get; set; }

        public ApplicationUser? ToUser { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
