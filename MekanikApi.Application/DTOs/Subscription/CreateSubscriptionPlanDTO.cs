using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Subscription
{
    public class CreateSubscriptionPlanDTO
    {
        public string Name { get; set; }

        public double Price { get; set; }
    }
}
