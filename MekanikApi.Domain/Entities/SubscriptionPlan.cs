using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public enum SubscriptionPlanTypes
    {
        None = 0,
        Monthly = 1,
        Quarterly = 2,
        Yearly = 3,
    }
    public class SubscriptionPlan
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double Price { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
