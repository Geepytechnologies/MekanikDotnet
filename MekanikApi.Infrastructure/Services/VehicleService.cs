using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Vehicle;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public async Task<GenericResponse> AddVehicle(VehicleDTO details, string accessToken)
        {
            try
            {
                var principal = _jwtService.GetTokenPrincipal(accessToken);

                if (principal is null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 403,
                        Message = "Error validating token"
                    };
                }

                var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var identityUser = await _userManager.FindByIdAsync(userId);
                var vehicleImages = new List<VehicleImage>();
                foreach (var image in details.Images)
                {
                    var uploadResult = FileService.UploadImageToCloudinary(image);
                    if (uploadResult.StatusCode == 200)
                    {
                        var uploadedImageUrl = (string)uploadResult.Result.Url;
                        var uploadedImageId = uploadResult.Result.Id;

                        var vehicleImage = new VehicleImage
                        {
                            ImageUrl = uploadedImageUrl,
                            CloudinaryPublicId = uploadedImageId,
                        };

                        vehicleImages.Add(vehicleImage);
                    }

                }
                _context.VehicleImages.AddRange(vehicleImages);
                await _context.SaveChangesAsync();

                var newvehicle = new Vehicle
                {
                    Make = details.Make,
                    Model = details.Model,
                    Year = details.Year,
                    PlateNumber = details.PlateNumber,
                    Vin = details.Vin,
                    EngineNumber = details.EngineNumber,
                    Color = details.Color,
                    RegNo = details.RegNo,
                    RegExpDate = details.RegExpDate,
                    UserId = identityUser.Id,
                    VehicleImages = vehicleImages,
                    CreatedAt = DateTime.UtcNow
                };

                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "Vehicle added successfully",
                    Result = newvehicle
                };
            }
            catch (Exception)
            {

                throw;
            }
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
