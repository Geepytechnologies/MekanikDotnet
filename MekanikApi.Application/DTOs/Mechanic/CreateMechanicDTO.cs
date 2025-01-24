using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Mechanic
{
    
    public class CreateMechanicDTO
    {
        public string? Name { get; set; }

        public string? Address { get; set; }

        public int Experience { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DayOfWeek[]? WorkDays { get; set; }

        public int StartHour { get; set; }

        public string? StartMeridien { get; set; }

        public int EndHour { get; set; }

        public string? EndMeridien { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public IFormFile? Image { get; set; }

        public Guid UserId { get; set; }

        public Guid[]? VehicleSpecialization { get; set; } 
        public Guid[]? ServiceSpecialization { get; set; }
    }
}
