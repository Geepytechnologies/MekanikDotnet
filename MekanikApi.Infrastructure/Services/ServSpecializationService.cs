using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Services
{
    public class ServSpecializationService : IServSpecializationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServSpecializationService> _logger;
        private readonly IJwtService _jwtService;
        public ServSpecializationService(ApplicationDbContext context, ILogger<ServSpecializationService> logger, IJwtService jwtService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _jwtService = jwtService;
            _userManager = userManager;
        }

        public async Task<GenericResponse> GetServiceSpecializations()
        {
            try
            {
                var services = await _context.Services.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description
                }).ToListAsync();

                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "successful",
                    Result = services
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving servSpecializations: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Error retrieving servicespecializations",
                };
                throw;
            }
        }
        
    }
}
