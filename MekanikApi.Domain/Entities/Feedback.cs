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

        public string Message { get; set; }

        public Guid FromUserId { get; set; }

        public User FromUser { get; set; }

        public Guid ToUserId { get; set; }

        public User ToUser { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
