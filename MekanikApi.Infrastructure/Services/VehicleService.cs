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
    public class VehicleService: IVehicleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VehicleService> _logger;
        private readonly IJwtService _jwtService;
        public VehicleService(ApplicationDbContext context, ILogger<VehicleService> logger, IJwtService jwtService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _jwtService = jwtService;
            _userManager = userManager;
        }

        public async Task<GenericResponse> GetVehicleSpecializations()
        {
            try
            {
                var mechanics = await _context.VehicleBrands.Select(v => new
                {
                    v.Id,
                    v.Name,
                    v.LogoUrl
                }).ToListAsync();

                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "VehicleBrands retrieved successfully",
                    Result = mechanics
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving vehiclebrands: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Error retrieving brands",
                };
                throw;
            }
        }
    }
}
