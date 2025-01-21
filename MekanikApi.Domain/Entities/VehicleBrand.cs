using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class VehicleBrand
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Many-to-Many Relationship
        public ICollection<Mechanic>? Mechanics { get; set; }

    }
}
