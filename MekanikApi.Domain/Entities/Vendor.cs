using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class Vendor
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public Point? Location { get; set; }
        public string? Image { get; set; }

        public Guid? SubscriptionPlanId { get; set; }

        public ICollection<Product>? Products { get; set; } = [];

        public Guid UserId { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
