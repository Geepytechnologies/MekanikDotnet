using MekanikApi.Application.DTOs.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Vehicle
{
    public class VehicleDTO
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? Year { get; set; }
        public string? PlateNumber { get; set; }
        public string? Vin { get; set; }
        public string? EngineNumber { get; set; }
        public string? Color { get; set; }

        public string? RegNo { get; set; }

        public string? RegExpDate { get; set; }

        public ICollection<IFormFile>? Images { get; set; }

    }
}
