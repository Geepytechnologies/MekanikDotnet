using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Mechanic;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Services
{
    public class MechanicService : IMechanicService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MechanicService> _logger;
        private readonly IJwtService _jwtService;
        public MechanicService(ApplicationDbContext context, ILogger<MechanicService> logger, IJwtService jwtService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _jwtService = jwtService;
            _userManager = userManager;
        }
        public async Task<GenericResponse> CreateMechanicProfile(CreateMechanicDTO details, string accessToken)
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

                if (identityUser is null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 404,
                        Message = "User not found"
                    };
                }
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var uploadedImageUrl = "";
                var uploadedImageId = "";
                if (details.Image != null)
                {
                    var uploadResult = FileService.UploadImageToCloudinary(details.Image);
                    if (uploadResult.StatusCode == 200)
                    {
                        uploadedImageUrl = (string)uploadResult.Result.Url;
                        uploadedImageId = uploadResult.Result.Id;
                        

                    }

                }

                var vehicleSpecializations = await _context.VehicleBrands
            .Where(v => details.VehicleSpecialization.Contains(v.Id))
                .ToListAsync();

                var serviceSpecializations = await _context.Services
                    .Where(s => details.ServiceSpecialization.Contains(s.Id))
                    .ToListAsync();
                var newMechanic = new Mechanic
                {
                    UserId = identityUser.Id,
                    Name = details.Name,
                    Address = details.Address,
                    Experience = details.Experience,
                    WorkDays = details.WorkDays,
                    StartHour = details.StartHour,
                    StartMeridien = details.StartMeridien,
                    EndHour = details.EndHour,
                    EndMeridien = details.EndMeridien,
                    Location = geometryFactory.CreatePoint(new Coordinate(details.Longitude, details.Latitude)),
                    Image = uploadedImageUrl,
                    VehicleSpecialization = vehicleSpecializations,
                    ServiceSpecialization = serviceSpecializations

                };
                await _context.Mechanics.AddAsync(newMechanic);
                await _context.SaveChangesAsync();

                return new GenericResponse
                {
                    StatusCode = 201,
                    Message = "Mechanic profile created successfully",
                };

            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating mechanic profile: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Error creating mechanic profile",
                };
                throw;
            }
        }

        public async Task<GenericResponse> FindMechanicsNearMe(double latitude, double longitude)
        {
            try
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var myLocation = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
                var mechanics = _context.Mechanics
                    .OrderBy(m => m.Location.Distance(myLocation))
                    .Where(m => m.Location.IsWithinDistance(myLocation, 2000))
                    .Select(m => new
                    {
                        m.Name,
                        m.Address,
                        m.Experience,
                        m.WorkDays,
                        Distance = m.Location.Distance(myLocation)
                    })
                    .ToList();

                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "Mechanics near you retrieved successfully",
                    Result = mechanics
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting mechanics near you: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Error getting mechanics near you",
                };
                throw;
            }
        }

        public async Task<GenericResponse> GetAllMechanics()
        {
            try
            {
                var mechanics = await _context.Mechanics.Include(m => m.VehicleSpecialization)
                    .Include(m => m.ServiceSpecialization)
                    .Select(mechanic => new MechanicResponseDTO(
                        mechanic.UserId,
                        mechanic.Name,
                        mechanic.Address,
                        mechanic.Experience,
                        mechanic.CarsFixed,
                        mechanic.ResponseTime,
                        mechanic.WorkDays,
                        mechanic.StartHour,
                        mechanic.StartMeridien,
                        mechanic.EndHour,
                        mechanic.EndMeridien,
                        mechanic.UserType,
                        mechanic.VerificationStatus,
                        mechanic.Image,
                        mechanic.Location != null ? mechanic.Location.Y : 0,
                        mechanic.Location != null ? mechanic.Location.X : 0,
                        mechanic.VehicleSpecialization.Select(v => v.Name).ToArray(),
                        mechanic.ServiceSpecialization.Select(s => s.Name).ToArray()
                    ))
                    .ToListAsync();


                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "Mechanics retrieved successfully",
                    Result = mechanics
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving mechanics: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Error retrieving mechanics",
                };
                throw;
            }
        }

        public Task<GenericResponse> GetMechanicProfile(Guid mechanicId)
        {
            throw new NotImplementedException();
        }
    }
}
