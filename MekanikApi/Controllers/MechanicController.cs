using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Mechanic;
using MekanikApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MekanikApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MechanicController : ControllerBase
    {
        private readonly IMechanicService _mechanicService;
        private readonly ILogger<MechanicController> _logger;
        public MechanicController(IMechanicService mechanicService, ILogger<MechanicController> logger)
        {
            _mechanicService = mechanicService;
            _logger = logger;
        }

        [HttpPost("CreateProfile")]
        public async Task<IActionResult> CreateMechanicProfile([FromForm] CreateMechanicDTO details)
        {
            try
            {
                var result = await _mechanicService.CreateMechanicProfile(details);
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

        // GET api/<MechanicController>/5
        [HttpGet]
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

        // POST api/<MechanicController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MechanicController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MechanicController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
