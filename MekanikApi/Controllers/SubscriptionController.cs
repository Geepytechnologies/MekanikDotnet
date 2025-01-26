using MekanikApi.Api.Extensions;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Subscription;
using MekanikApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MekanikApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;    
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreatePlans")]
        public async Task<IActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanDTO details)
        {
            try
            {
                var accessToken = HttpContext.GetAuthorizationHeader();
                var response = await _subscriptionService.CreateSubscriptionPlan(details, accessToken);
                return StatusCode(response.StatusCode, new ApiResponse
                {
                    StatusCode = response.StatusCode,
                    Message = response.Message,
                    Result = response.Result
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while processing your request"
                });
                throw;
            }
            
        }
    }
}
