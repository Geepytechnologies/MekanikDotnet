using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.Interfaces;
using MekanikApi.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;



namespace MekanikApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServSpecializationService _servSpecializationService;
        private readonly ILogger<ServiceController> _logger;
        public ServiceController(IServSpecializationService servSpecializationService, ILogger<ServiceController> logger)
        {
            _servSpecializationService = servSpecializationService;
            _logger = logger;
        }


        [HttpGet("specializations")]
        public async Task<IActionResult> GetAllMechanics()
        {
            try
            {
                var result = await _servSpecializationService.GetServiceSpecializations();
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

        // GET api/<ServiceSpecializationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ServiceSpecializationController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ServiceSpecializationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ServiceSpecializationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
