using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class Mechanic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int Experience { get; set; }

        public int WorkDays { get; set; }

        public Point Location { get; set; }

        public IFormFile Image { get; set; }
    }
}
