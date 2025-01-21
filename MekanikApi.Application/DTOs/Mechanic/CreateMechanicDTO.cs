using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Mechanic
{
    public class CreateMechanicDTO
    {
        public string? Name { get; set; }

        public string? Address { get; set; }

        public int Experience { get; set; }

        public DayOfWeek[]? WorkDays { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string? Image { get; set; }
    }
}
