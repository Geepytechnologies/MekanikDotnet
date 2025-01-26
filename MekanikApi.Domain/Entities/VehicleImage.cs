using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class VehicleImage
    {
        public Guid Id { get; set; }
        public string? ImageUrl { get; set; }

        public string? CloudinaryPublicId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

    }
}
