using MekanikApi.Domain.Entities;
using MekanikApi.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Mechanic
{
    public class UpdateMechanicDTO
    {
        public string? Name { get; set; }

        public string? Address { get; set; }

        public int? Experience { get; set; }

        public DayOfWeek[]? WorkDays { get; set; } = [];

        public int? StartHour { get; set; }

        public string? StartMeridien { get; set; }

        public int? EndHour { get; set; }

        public string? EndMeridien { get; set; }

   

        public IFormFile? Image { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }


    }
}
