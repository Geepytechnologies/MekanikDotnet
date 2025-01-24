using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Vendor
{
    public class CreateVendorDTO
    {
        public string? Name { get; set; }

        public string? Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public IFormFile? Image { get; set; }

        
    }
}
