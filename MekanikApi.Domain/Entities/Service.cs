using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    // Type of service eg Repair Service, Maintenance Service
    public class Service
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Many-to-Many Relationship
        public ICollection<Mechanic>? Mechanics { get; set; }

    }
}
