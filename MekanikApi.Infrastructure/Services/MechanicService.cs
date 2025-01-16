using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Mechanic;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Services
{
    public class MechanicService : IMechanicService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MechanicService> _logger;
        public MechanicService(ApplicationDbContext context, ILogger<MechanicService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<GenericResponse> CreateMechanicProfile(CreateMechanicDTO details)
        {
            try
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var newMechanic = new Mechanic
                {
                    Name = details.Name,
                    Address = details.Address,
                    Experience = details.Experience,
                    WorkDays = details.WorkDays,
                    Location = geometryFactory.CreatePoint(new Coordinate(details.Longitude, details.Latitude)),
                    Image = details.Image
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
                var mechanics = await _context.Mechanics.FindAsync();

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
