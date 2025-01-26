using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Subscription;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Services
{
    public class SubscriptionService: ISubscriptionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(ApplicationDbContext context, ILogger<SubscriptionService> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<GenericResponse> CreateSubscriptionPlan(CreateSubscriptionPlanDTO details, string accessToken)
        {
            try
            {
                var plan = new SubscriptionPlan
                {
                    Name = details.Name,
                    Price = details.Price
                };
                await _context.SubscriptionPlans.AddAsync(plan);
                await _context.SaveChangesAsync();

                return new GenericResponse
                {
                    StatusCode = 201,
                    Message = "Subscription plan created",
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
