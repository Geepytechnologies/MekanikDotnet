using MekanikApi.Api.Extensions;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Mechanic;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using MekanikApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MekanikApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MechanicController : ControllerBase
    {
        private readonly IMechanicService _mechanicService;
        private readonly ILogger<MechanicController> _logger;
        private readonly ILocationService _locationService;
        public MechanicController(IMechanicService mechanicService, ILogger<MechanicController> logger, ILocationService locationService)
        {
            _mechanicService = mechanicService;
            _logger = logger;
            _locationService = locationService;
        }

        [Authorize]
        [HttpPost("CreateProfile")]
        public async Task<IActionResult> CreateMechanicProfile([FromForm] CreateMechanicDTO details)
        {
            try
            {
                var accessToken = HttpContext.GetAuthorizationHeader();
                var result = await _mechanicService.CreateMechanicProfile(details, accessToken);
                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Result = result.Result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating mechanic profile: {msg}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                });
                throw;
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMechanics()
        {
            try
            {
                var result = await _mechanicService.GetAllMechanics();
                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Result = result.Result
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                });
                throw;
            }
        }

        //[Authorize]
        [HttpGet("profile/{mechanicId}")]
        public async Task<IActionResult> GetAMechanic(Guid mechanicId)
        {
            try
            {
                var res = await _mechanicService.GetMechanicProfile(mechanicId);
                return StatusCode(res.StatusCode, new ApiResponse
                {
                    StatusCode = res.StatusCode,
                    Message = res.Message,
                    Result = res.Result
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                });
                throw;
            }
        }
        //[Authorize]
        [HttpGet("NearbyMechanics")]
        [SwaggerOperation(Summary = "Returns Mechanics", Description = "Gets Nearby Mechanics")]
        public async Task<IActionResult> GetMechanicsNearMe(double latitude, double longitude)
        {
            try
            {
                
                var result = await _mechanicService.FindMechanicsNearMe(latitude, longitude);
                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Result = result.Result
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                });
                throw;
            }
        }

        // POST api/<MechanicController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [Authorize]
        [HttpPatch("update")]
        public async Task<IActionResult> UpdateMechanic([FromForm] UpdateMechanicDTO details)
        {
            try
            {
                var accessToken = HttpContext.GetAuthorizationHeader();
                var res = await _mechanicService.UpdateAMechanic(details, accessToken);
                return StatusCode(res.StatusCode, new ApiResponse
                {
                    StatusCode = res.StatusCode,
                    Message = res.Message,
                    Result = res.Result
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error updating user: {msg}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Something went wrong",
                });
            }
        }

        [HttpGet("Calculate/TravelTime")]
        public async Task<IActionResult> GetTravelTime()
        {
            var result = await _locationService.GetTravelTimeAsync(5.532003, 7.486027, 5.4850, 7.0354);
            return Ok(result);
        }

        // DELETE api/<MechanicController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
