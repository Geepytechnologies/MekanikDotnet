using MekanikApi.Api.Extensions;
using MekanikApi.Application.DTOs.Auth.Requests;
using MekanikApi.Application.DTOs.Auth.Responses;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Sms;
using MekanikApi.Application.Interfaces;
using MekanikApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;



namespace MekanikApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger, IAuthService authService)
        {
            _userService = userService;
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericTypeResponse<RegisterResponse>))]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Register A User", Description = "Register A Mechanic User")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO user)
        {
            _logger.LogInformation("Registration credentials: {msg}", JsonConvert.SerializeObject(user));
            try
            {
                var result = await _authService.RegisterUser(user);
                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Result = result.Result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Registration Error: {msg}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Something went wrong",
                    Result = null
                });
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericTypeResponse<UserLoginResponseDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GenericTypeResponse<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(GenericTypeResponse<UserNotVerifiedResponseDTO>))]
        [SwaggerOperation(Summary = "Login", Description = "Give A User Access to the Application")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO user)
        {
            try
            {
                _logger.LogInformation("login credentials: {msg}", JsonConvert.SerializeObject(user));
                
                var result = await _authService.Login(user);

                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Result = result.Result
                });
                
            }
            catch (Exception ex)
            {
                _logger.LogError("login Error: {msg} ", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Something went wrong",
                    Result = null
                });
            }
        }

        [HttpPost("VerifyOtp")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Verify Otp", Description = "Verify Otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] ConfirmOtpDTO details)
        {
            try
            {
                
                var result = await _authService.ConfirmOtp(details);
                var response = new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Result = result.Result
                };
                

                return StatusCode(result.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Verify Otp Error: {msg}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Result = null
                });
            }
        }


        [HttpPost("ForgotPassword")]
        [SwaggerOperation(Summary = "Complete the Forgot Password Process", Description = "This will confirm the otp sent to the user and also create a new password for the user")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO details)
        {
            try
            {
                var result = await _authService.ForgotPassword(details);
                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("forgot password Error: {msg}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Result = null
                });
                throw;
            }
        }

        /// <summary>
        /// Resets the user's password.
        /// </summary>
        [Authorize]
        [HttpPost("ResetPassword")]
        [SwaggerOperation(Summary = "Reset password", Description = "Requires Authorization header with Bearer token.")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO details)
        {

            try
            {
                var accessToken = HttpContext.GetAuthorizationHeader();
                var result = await _authService.ResetPassword(details, accessToken);
                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("forgot password Error: {msg}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Result = null
                });
                throw;
            }
        }

        [HttpPost("RefreshToken")]
        [SwaggerOperation(Summary = "Refresh Token", Description = "Get a new access token by using the refresh token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
        {
            try
            {
                var result = await _authService.RefreshToken(model);
                return StatusCode(result.StatusCode, new ApiResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Result = result.Result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Result = null
                });
            }
        }
    }
}
