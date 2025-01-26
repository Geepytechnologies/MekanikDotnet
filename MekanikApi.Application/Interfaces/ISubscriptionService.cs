using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<GenericResponse> CreateSubscriptionPlan(CreateSubscriptionPlanDTO details, string accessToken);
    }
}
