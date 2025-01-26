using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? Year { get; set; }
        public string? PlateNumber { get; set; }
        public string? Vin { get; set; }
        public string? EngineNumber { get; set; }
        public string? Color { get; set; }

        public string? RegNo { get; set; }

        public string? RegExpDate { get; set; }
        public ICollection<VehicleImage>? VehicleImages { get; set; } = [];
        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
