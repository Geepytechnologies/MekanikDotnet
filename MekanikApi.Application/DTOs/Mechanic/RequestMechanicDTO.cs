using MekanikApi.Domain.Entities;
using MekanikApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Mechanic
{
    public class RequestMechanicDTO
    {
        public Guid ServiceId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid RequestedForId { get; set; }
    }
}
