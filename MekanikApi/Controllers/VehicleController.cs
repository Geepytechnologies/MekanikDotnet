using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MekanikApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehicleController> _logger;
        public VehicleController(IVehicleService vehicleService, ILogger<VehicleController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [HttpGet("specializations")]
        public async Task<IActionResult> GetAllVehicleSpecializations()
        {
            try
            {
                var result = await _vehicleService.GetVehicleSpecializations();
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

        // GET api/<VehicleController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<VehicleController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<VehicleController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<VehicleController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
