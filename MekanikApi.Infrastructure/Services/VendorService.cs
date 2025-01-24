using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Mechanic;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MekanikApi.Application.DTOs.Vendor;
using Microsoft.EntityFrameworkCore;

namespace MekanikApi.Infrastructure.Services
{
    public class VendorService: IVendorService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VendorService> _logger;
        private readonly IJwtService _jwtService;

        public VendorService(ApplicationDbContext context, ILogger<VendorService> logger, IJwtService jwtService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _jwtService = jwtService;
            _userManager = userManager;
        }

        public async Task<GenericResponse> CreateVendorProfile(CreateVendorDTO details, string accessToken)
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
                var newMechanic = new Mechanic
                {
                    UserId = identityUser.Id,
                    Name = details.Name,
                    Address = details.Address,
                    Location = geometryFactory.CreatePoint(new Coordinate(details.Longitude, details.Latitude)),
                    Image = uploadedImageUrl
                };
                await _context.Mechanics.AddAsync(newMechanic);
                await _context.SaveChangesAsync();

                return new GenericResponse
                {
                    StatusCode = 201,
                    Message = "Vendor profile created successfully",
                };

            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating vendor profile: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Error creating vendor profile",
                };
                throw;
            }
        }

        public async Task<GenericResponse> FindVendorsNearMe(double latitude, double longitude)
        {
            try
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var myLocation = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
                var vendors = _context.Vendors
                    .OrderBy(m => m.Location.Distance(myLocation))
                    .Where(m => m.Location.IsWithinDistance(myLocation, 2000))
                    .Select(m => new
                    {
                        m.Name,
                        m.Address,
                        m.Image,
                        Distance = m.Location.Distance(myLocation)
                    })
                    .ToList();

                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "vendors near you retrieved successfully",
                    Result = vendors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting vendors near you: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Error getting vendors near you",
                };
                throw;
            }
        }

        public async Task<GenericResponse> GetAllVendors()
        {
            try
            {
                var mechanics = await _context.Vendors.ToListAsync();

                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "Vendors retrieved successfully",
                    Result = mechanics
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving vendors: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Error retrieving vendors",
                };
                throw;
            }
        }

        public Task<GenericResponse> GetVendorProfile(Guid vendorId)
        {
            throw new NotImplementedException();
        }
    }
}

