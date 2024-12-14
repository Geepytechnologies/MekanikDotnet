using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using MekanikApi.Application.DTOs.Auth.Requests;
using MekanikApi.Application.DTOs.Auth.Responses;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Sms;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using MekanikApi.Domain.Interfaces;
using MekanikApi.Infrastructure.DataContext;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MekanikApi.Infrastructure.Services
{
    public class AuthService : IAuthService

    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ISmsService _smsService;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(UserManager<User> userManager, IConfiguration config, ILogger<AuthService> logger, ApplicationDbContext context, IUserService userService, IMapper mapper, SignInManager<User> signInManager, ISmsService smsService, IJwtService jwtService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _config = config;
            _logger = logger;
            _context = context;
            _userService = userService;
            _mapper = mapper;
            _signInManager = signInManager;
            _smsService = smsService;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponse> Login(LoginRequestDTO user)
        {
            
            var identityuser = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.Phone);
            if (identityuser is null)
            {
                return new LoginResponse
                {
                    Status = false,
                    Message = "user not found"
                };
            }
            var roles = await _userManager.GetRolesAsync(identityuser);
           

            var result = await _signInManager.PasswordSignInAsync(identityuser, user.Password, isPersistent: true, lockoutOnFailure: true);
            _logger.LogInformation("signinmanagerResult: {data}", result.ToString());

            if (result.Succeeded)
            {
              

               
                var _applicationUser = await _context.Users
                    .Where(u => u.PhoneNumber == user.Phone)
                    .FirstOrDefaultAsync();

                //if (identityuser.IsVerified)
                //{
                //    var tokenstring = JwtService.GenerateAccessTokenAsync(user.Phone, identityuser.Id, roles);
                //    var refreshToken = JwtService.GenerateRefreshToken(user.Phone);
                //    identityuser.RefreshToken = refreshToken;
                //    identityuser.RefreshTokenExpiry = DateTime.Now.AddHours(2);
                //    identityuser.DeviceHash = deviceHash;
                //    identityuser.PushToken = user.PushToken;

                //    if (identityuser.DeviceHash == string.Empty)
                //    {
                //        identityuser.DeviceHash = deviceHash;
                //    }

                //    await UpdateUser(identityuser);



                //    var userDetails = new UserLoginResponseDTO(
                //        UserId: identityuser.Id,
                //        FirstName: identityuser.Firstname,
                //        MiddleName: identityuser.Middlename,
                //        LastName: identityuser.Lastname,
                //        PhoneNumber: identityuser.PhoneNumber,
                //        Email: identityuser.Email,
                //        BusinessName: identityuser.BusinessName,
                //        HomeAddress: identityuser.Homeaddress,
                //        DateOfBirth: identityuser.DateOfBirth,
                //        LGA: identityuser.LGA,
                //        StateOfOrigin: identityuser.StateOfOrigin,
                //        ImageUrl: identityuser.ImageUrl,
                //        Kyc: identityuser.KYC,
                //        AccountPinSet: identityuser.AccountPinSet,
                //        AccessToken: tokenstring,
                //        RefreshToken: refreshToken
                //        )

                //    ;
                //    return new LoginResponse
                //    {
                //        Status = true,
                //        Message = "Login successful",
                //        UserDetails = userDetails
                //    };
                //}
                //else
                //{
                //    return new LoginResponse
                //    {
                //        Status = false,
                //        Message = "User is not verified",
                //        UserNotVerifiedDetails = new UserNotVerifiedResponseDTO(identityuser.Firstname,identityuser.Email, identityuser.PhoneNumber)
                //    };
                //}
                return new LoginResponse
                {
                    Status = false,
                    Message = ""
                };
            }
            else
            {
                if (result.IsLockedOut)
                {
                    var userlockedoutresponse = new LoginResponse
                    {
                        Status = false,
                        Message = "Account locked. Please try again in 5 minutes",
                    };
                    return userlockedoutresponse;
                }
                return new LoginResponse
                {
                    Status = false,
                    Message = "Invalid phone or password."
                };
            }
        }

        
        public async Task<RegisterResponse> RegisterUser(RegisterRequestDTO user)
        {
            _logger.LogInformation("Register User Details: {msg}", JsonConvert.SerializeObject(user));
            var existingEmailUser = await _userManager.FindByEmailAsync(user.Email);
            var existingPhoneUser = await _userManager.FindByNameAsync(user.PhoneNumber);

            if (existingEmailUser is not null)
            {
                return new RegisterResponse
                {
                    Status = false,
                    Message = "Email already exists"
                };
            }
            if (existingPhoneUser is not null)
            {
                return new RegisterResponse
                {
                    Status = false,
                    Message = "Phone already exists"
                };
            }

            try
            {
                var identityUser = new User
                {
                    UserName = user.PhoneNumber,
                    Email = user.Email,
                    
                };

                var result = await _userManager.CreateAsync(identityUser, user.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(identityUser, "User");
                    var userId = identityUser.Id;

                    //await _accountService.CreateAccount(user.PhoneNumber, userId);

                    return new RegisterResponse
                    {
                        Status = true,
                        Message = "User created successfully"
                    };
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogInformation("User Manager Error: {msg}", error.Description);
                    }
                    return new RegisterResponse
                    {
                        Status = false,
                        Message = "user registration failed"
                    };
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    errorMessage += $" Inner Exception: {ex.Message}";
                }
                _logger.LogInformation("Registration Error: {msg}", errorMessage);

                return new RegisterResponse
                {
                    Status = false,
                    Message = "User registration failed"
                };
            }
        }
        public async Task<GenericResponse> ForgotPassword(ForgotPasswordDTO user)
        {
            try
            {
                //var verifyotpdetails = new ConfirmOtpDTO
                //{
                //    MobileNumber = user.MobileNumber,
                //    Otp = user.Otp,
                //    PinId = user.PinId
                //};
                //var verificationResult = await _smsService.VerifyOTP(verifyotpdetails);

                //if (verificationResult.Result.Verified != "True")
                //{
                //    return new GenericResponse
                //    {
                //        StatusCode = 403,
                //        Message = $"Otp verification failure: {verificationResult.Result.Verified}"
                //    };
                //}
                var identityuser = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.MobileNumber);

                if (identityuser is null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 404,
                        Message = "user not found"
                    };
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(identityuser);
                var result = await _userManager.ResetPasswordAsync(identityuser, token, user.Password);
                if (result.Succeeded)
                {
                    return new GenericResponse
                    {
                        StatusCode = 200,
                        Message = "Password Change Successful",
                    };

                }
                else
                {
                    return new GenericResponse
                    {
                        StatusCode = 400,
                        Message = "Password Change Unsuccessful",
                    };
                }
            
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error changing password: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Something Went Wrong",
                };
                throw;
            }

        }
        
         public async Task<GenericResponse> ResetPassword(ResetPasswordDTO user, string accessToken)
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
                
                
                var isValidPassword = await _userManager.CheckPasswordAsync(identityUser, user.OldPassword);
                if (!isValidPassword)
                {
                    return new GenericResponse
                    {
                        StatusCode = 400,
                        Message = "Invalid Old Password"
                    };
                }

                // Update password
                var result = await _userManager.ChangePasswordAsync(identityUser, user.OldPassword, user.NewPassword);
                
                if (result.Succeeded)
                {
                    return new GenericResponse
                    {
                        StatusCode = 200,
                        Message = "Password Change Successful",
                    };

                }
                else
                {
                    return new GenericResponse
                    {
                        StatusCode = 400,
                        Message = "Password Change Unsuccessful",
                    };
                }
            
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error changing password: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Something Went Wrong",
                };
                throw;
            }

        }
        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User> GetUser(string email)
        {
            var identityuser = await _userManager.FindByEmailAsync(email);
            return identityuser;
        }

        public async Task<GenericResponse> RefreshToken(RefreshTokenModel model)
        {
            try
            {
                var principal = GetTokenPrincipal(model.AccessToken);
                var response = new RefreshResponse();

                if (principal?.Identity?.Name is null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 403,
                        Message = "Unauthorized operation"
                    };
                }
                var userphone = principal.Identity.Name;
                var identityUser = await _context.Users.Where(u => u.PhoneNumber == userphone).FirstOrDefaultAsync();

                //if (identityUser is null || identityUser.RefreshToken != model.RefreshToken)
                //{
                //    return new GenericResponse
                //    {
                //        StatusCode = 401,
                //        Message = "Invalid token"
                //    };
                //}
                var roles = await _userManager.GetRolesAsync(identityUser);

               
                var phone = identityUser.PhoneNumber;
                var accessToken = JwtService.GenerateAccessTokenAsync(phone, identityUser.Id, roles);
                var refreshToken = JwtService.GenerateRefreshToken(phone);
                //identityUser.RefreshToken = refreshToken;

                await UpdateUser(identityUser);
                var refreshResponse = new
                {
                    accessToken,
                    refreshToken
                };
                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "Successful",
                    Result = refreshResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error from refreshToken: {msg}", ex.Message);
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Internal server error"
                };
                throw;
            }
            

            
            
        }

        private ClaimsPrincipal GetTokenPrincipal(string accessToken)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTKEY")));
            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securitykey,
                ValidateLifetime = true,
                ValidateActor = true,
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWTISSUER"),
                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("JWTAUDIENCE")
            };
            try
            {
                return new JwtSecurityTokenHandler().ValidateToken(accessToken, validation, out _);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation error: {ex.Message}");
                return null;
            }
        }
    }
}