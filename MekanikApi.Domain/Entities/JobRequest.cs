using MekanikApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class JobRequest
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Service? Service { get; set; }

        public Guid VehicleId { get; set; }

        public Vehicle? Vehicle { get; set; }

        public JobRequestStatus Status { get; set; } = JobRequestStatus.Pending;
        public Guid RequestedFromId { get; set; }
        public ApplicationUser? RequestedFrom { get; set; }

        public Guid RequestedForId { get; set; }
        public ApplicationUser? RequestedFor { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
