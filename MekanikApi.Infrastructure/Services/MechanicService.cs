using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Mechanic;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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
        private readonly ILocationService _locationService;
        public MechanicService(ApplicationDbContext context, ILogger<MechanicService> logger, IJwtService jwtService, UserManager<ApplicationUser> userManager, ILocationService locationService)
        {
            _context = context;
            _logger = logger;
            _jwtService = jwtService;
            _userManager = userManager;
            _locationService = locationService;
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
                var existingMechanicProfile = await _context.Mechanics.Where(m => m.UserId == identityUser.Id).FirstOrDefaultAsync();

                if(existingMechanicProfile is not null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 409,
                        Message = "Mechanic Profile Already Exists"
                    };
                }
                identityUser.Profile = [.. identityUser.Profile, Domain.Enums.ApplicationProfile.MECHANIC];
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
                    CloudinaryImageId = uploadedImageId,
                    VehicleSpecialization = vehicleSpecializations,
                    ServiceSpecialization = serviceSpecializations

                };
                _context.ApplicationUsers.Update(identityUser);
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

                // Fetch mechanics within 2km radius from the database first
                var mechanicsQuery = _context.Mechanics
                    .OrderBy(m => m.Location.Distance(myLocation))
                    .Where(m => m.Location.IsWithinDistance(myLocation, 2000))
                    .Select(m => new
                    {
                        m.Id,
                        m.Name,
                        m.Address,
                        m.Experience,
                        m.WorkDays,
                        m.Image,
                        m.UserType,
                        Distance = m.Location.Distance(myLocation),
                        Latitude = m.Location.Y,
                        Longitude = m.Location.X
                    })
                    .ToList();

                // Calculate travel times for each mechanic
                var mechanics = new List<object>();
                foreach (var mechanic in mechanicsQuery)
                {
                    var travelTime = await _locationService.GetTravelTimeAsync(
                        latitude,
                        longitude,
                        mechanic.Latitude,
                        mechanic.Longitude
                    );

                    mechanics.Add(new
                    {
                        mechanic.Id,
                        mechanic.Name,
                        mechanic.Address,
                        mechanic.Experience,
                        mechanic.WorkDays,
                        mechanic.Distance,
                        mechanic.Image,
                        mechanic.UserType,
                        Time = travelTime
                    });
                }

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
                    Message = "Error getting mechanics near you"
                };
            }
        }


        public async Task<GenericResponse> GetAllMechanics()
        {
            try
            {
                var mechanics = await _context.Mechanics.Include(m => m.VehicleSpecialization)
                    .Include(m => m.ServiceSpecialization)
                    .Select(mechanic => new MechanicResponseDTO(
                        mechanic.Id,
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

        public async Task<GenericResponse> GetMechanicProfile(Guid mechanicId)
        {
            try
            {
                var mechanic = await _context.Mechanics
                    .Include(m => m.VehicleSpecialization)
                    .Include(m => m.ServiceSpecialization)
                    .Where(m => m.Id == mechanicId)
                    .Select(mechanic => new MechanicResponseDTO(
                        mechanic.Id,
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

                if (mechanic is null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 404,
                        Message = "Mechanic not found"
                    };
                }
               
                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "Successful",
                    Result = mechanic
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching mechanic: {msg}", ex.Message);
                throw;
            }
        }

        public async Task<GenericResponse> UpdateAMechanic(UpdateMechanicDTO details, string accessToken)
        {
            try
            {
                if (details == null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 400,
                        Message = "Details cannot be null."
                    };
                }

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
                var mechanic = await _context.Mechanics
                    .Include(m => m.VehicleSpecialization)
                    .Include(m => m.ServiceSpecialization)
                    .FirstOrDefaultAsync(m => m.UserId == identityUser.Id);

                if (mechanic == null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 404,
                        Message = "Mechanic not found."
                    };
                }

                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

                if (details.Image != null)
                {
                    var uploadResult = FileService.UploadImageToCloudinary(details.Image);
                    if (uploadResult.StatusCode == 200 && uploadResult.Result != null)
                    {
                        mechanic.Image = uploadResult.Result.Url;
                        mechanic.CloudinaryImageId = uploadResult.Result.Id;
                    }
                }

                if (!string.IsNullOrWhiteSpace(details.Name))
                    mechanic.Name = details.Name;

                if (!string.IsNullOrWhiteSpace(details.Address))
                    mechanic.Address = details.Address;

                if (details.Experience > 0)
                    mechanic.Experience = (int)details.Experience;

                if (details.WorkDays != null)
                    mechanic.WorkDays = details.WorkDays;

                if (details.StartHour > 0)
                    mechanic.StartHour = (int)details.StartHour;

                if (details.EndHour > 0)
                    mechanic.EndHour = (int)details.EndHour;

                if (!string.IsNullOrWhiteSpace(details.StartMeridien))
                    mechanic.StartMeridien = details.StartMeridien;

                if (!string.IsNullOrWhiteSpace(details.EndMeridien))
                    mechanic.EndMeridien = details.EndMeridien;

                if (details.Latitude >= -90 && details.Latitude <= 90 &&
                    details.Longitude >= -180 && details.Longitude <= 180)
                {
                    mechanic.Location = geometryFactory.CreatePoint(new Coordinate((double)details.Longitude, (double)details.Latitude));
                }

                _context.Mechanics.Update(mechanic);
                await _context.SaveChangesAsync();

                var vehicleSpecializations = mechanic.VehicleSpecialization?.Select(v => v.Name).ToArray() ?? [];
                var serviceSpecializations = mechanic.ServiceSpecialization?.Select(s => s.Name).ToArray() ?? [];

                var mechanicResponse = new MechanicResponseDTO(
                    mechanic.Id,
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
                    mechanic.Location?.Y ?? 0,
                    mechanic.Location?.X ?? 0,
                    vehicleSpecializations,
                    serviceSpecializations
                );

                return new GenericResponse
                {
                    StatusCode = 201,
                    Message = "Update successful.",
                    Result = mechanicResponse
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating mechanic: {ex.Message}");
                throw;
            }
        }


        public async Task<GenericResponse> RequestForAMechanic(RequestMechanicDTO details, string accessToken)
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

                var jobrequest = new JobRequest
                {
                    ServiceId = details.ServiceId,
                    VehicleId = details.VehicleId,
                    RequestedFromId = identityUser.Id,
                    RequestedForId = details.RequestedForId,
                };
                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "Request sent successfully",
                    Result = jobrequest
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    
        
    }
}
