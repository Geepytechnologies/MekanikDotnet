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
using Google.Apis.Auth;
using System.Security.Cryptography;
using MekanikApi.Domain.Enums;

namespace MekanikApi.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
        private readonly ICacheService _cacheService;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;


        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, ILogger<AuthService> logger, ICacheService cacheService, IEmailService emailService, IJwtService jwtService, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _config = config;
            _logger = logger;
            _cacheService = cacheService;
            _emailService = emailService;
            _jwtService = jwtService;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        private static string GenerateOtp()
        {
            Random random = new();
            return random.Next(100000, 999999).ToString();
        }
        private static string HashValue(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(SHA256.HashData(bytes));
        }
        public static async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string accessToken)
        {
            try
            {
                var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                if (string.IsNullOrEmpty(googleClientId))
                {
                    throw new InvalidOperationException("Google Client ID is not configured.");
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { googleClientId }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken, settings);
                return payload;
            }
            catch (InvalidJwtException)
            {
                // Handle invalid token
                return null;
            }
        }
    


        public async Task<bool> SendVerificationCode(string email, string otp, string firstname)
        {
            var body = await EmailService.GetEmailBodyAsync("verification.html");
            var emailbody = body.Replace("{{OTP_CODE}}", otp).Replace("{{NAME}}", firstname);
            var result = await _emailService.SendEmailAsync(email, "Mekanik Verification", emailbody);
            return result;
        }
        public async Task<GenericResponse> Login(LoginRequestDTO user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                return new GenericResponse
                {
                    StatusCode = 400,
                    Message = "Email is required."
                };
            }

            var identityuser = await _userManager.FindByEmailAsync(user.Email);

            if (identityuser is null)
            {
                return new GenericResponse
                {
                    StatusCode = 404,
                    Message = "user not found"
                };
            }

            var roles = await _userManager.GetRolesAsync(identityuser);
            var result = await _signInManager.PasswordSignInAsync(identityuser, user.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var _applicationUser = identityuser;
                if (_applicationUser.EmailConfirmed)
                {
                    var tokenstring = JwtService.GenerateAccessTokenAsync(user.Email, identityuser.Id, roles);
                    var refreshToken = JwtService.GenerateRefreshToken(user.Email);
                    var hashedRefreshToken = HashValue(refreshToken);
                    var storeTokenResult = await _userManager.SetAuthenticationTokenAsync(
                        _applicationUser,
                        "Local",
                        "RefreshToken",
                        refreshToken
                    );

                    _applicationUser.RefreshToken = hashedRefreshToken;
                    //_applicationUser.RefreshTokenExpiry = DateTime.Now.AddHours(2);

                    await _userManager.UpdateAsync(_applicationUser);

                    var userDetails = new UserLoginResponseDTO(
                        UserId: _applicationUser.Id,
                        FirstName: _applicationUser.Firstname,
                        MiddleName: _applicationUser.Middlename,
                        LastName: _applicationUser.Lastname,
                        PhoneNumber: _applicationUser.PhoneNumber,
                        Email: _applicationUser.Email,
                        Profile: _applicationUser.Profile,
                        AccessToken: tokenstring,
                        RefreshToken: refreshToken
                    );

                    return new GenericResponse
                    {
                        StatusCode = 200,
                        Message = "Login successful",
                        Result = userDetails
                    };
                }
                else
                {
                    string otp = GenerateOtp();
                    var hashedOtp = HashValue(otp);
                    var emailRes = await SendVerificationCode(user.Email, otp, _applicationUser.Firstname);

                    if (emailRes)
                    {
                        _applicationUser.EmailVerificationCode = hashedOtp;
                        _applicationUser.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

                        await _userManager.UpdateAsync(_applicationUser);

                        return new GenericResponse
                        {
                            StatusCode = 401,
                            Message = "User is not verified, Otp has been sent to email",
                        };
                    }
                    return new GenericResponse
                    {
                        StatusCode = 401,
                        Message = "User is not verified, Otp sending failed",
                    };
                }
            }
            else
            {
                if (result.IsLockedOut)
                {
                    var userlockedoutresponse = new GenericResponse
                    {
                        StatusCode = 403,
                        Message = "Account locked. Please try again in 5 minutes",
                    };
                    return userlockedoutresponse;
                }
                return new GenericResponse
                {
                    StatusCode = 400,
                    Message = "Invalid email or password."
                };
            }
        }

        public async Task<GenericResponse> RegisterUser(RegisterRequestDTO user)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser != null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 409,
                        Message = "User already exists"
                    };
                }

                string otp = GenerateOtp();
                var hashedOtp = HashValue(otp);
                var emailRes = await SendVerificationCode(user.Email, otp, user.Firstname);

                if (!emailRes)
                {
                    return new GenericResponse
                    {
                        StatusCode = 400,
                        Message = "Failed to send verification email"
                    };
                }

                var identityUser = new ApplicationUser
                {
                    UserName = user.Email,
                    Email = user.Email,
                    Firstname = user.Firstname,
                    Middlename = user.Middlename,
                    Lastname = user.Lastname,
                    PhoneNumber = user.PhoneNumber,
                    PushToken = user.PushToken,
                    EmailVerificationCode = hashedOtp,
                    OtpExpiry = DateTime.UtcNow.AddMinutes(10),
                    Profile = [user.Profile], 
                };

                var result = await _userManager.CreateAsync(identityUser, user.Password);

                if (result.Succeeded)
                {
                    var savedUser = await _userManager.FindByEmailAsync(user.Email);
                    if (savedUser != null)
                    {
                        if (!await _roleManager.RoleExistsAsync("User"))
                        {
                            var role = new IdentityRole<Guid> { Name = "User" };
                            await _roleManager.CreateAsync(role);
                        }
                        await _userManager.AddToRoleAsync(savedUser, "User");

                    }
                    return new GenericResponse
                    {
                        StatusCode = 201,
                        Message = "User created successfully"
                    };
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogInformation("Error in registration: {data}", error.ToString());
                        Console.WriteLine($"Error: {error.Code} - {error.Description}");
                    }

                    return new GenericResponse
                    {
                        StatusCode = 400,
                        Message = "User registration failed"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user");
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Internal server error",
                };
            }
        }

        public async Task<GenericResponse> GoogleAuth(GoogleRequestDTO user)
        {
            try
            {
                var _applicationUser = await _userManager.FindByEmailAsync(user.Email);
                var roles = await _userManager.GetRolesAsync(_applicationUser);


                if (_applicationUser != null)
                {

                    var tokenstring = JwtService.GenerateAccessTokenAsync(user.Email, _applicationUser.Id, roles);
                    var refreshToken = JwtService.GenerateRefreshToken(user.Email);
                    _applicationUser.RefreshToken = refreshToken;
                    _applicationUser.RefreshTokenExpiry = DateTime.Now.AddHours(2);
                    await _userManager.UpdateAsync(_applicationUser);

                    var userDetails = new UserLoginResponseDTO(
                        UserId: _applicationUser.Id,
                        FirstName: _applicationUser.Firstname,
                        MiddleName: _applicationUser.Middlename,
                        LastName: _applicationUser.Lastname,
                        PhoneNumber: _applicationUser.PhoneNumber,
                        Email: _applicationUser.Email,
                        Profile: _applicationUser.Profile,
                        AccessToken: tokenstring,
                        RefreshToken: refreshToken
                        );

                    return new GenericResponse
                    {
                        StatusCode = 200,
                        Message = "Successful",
                        Result = userDetails
                    };
                }

                var identityUser = new ApplicationUser
                {
                    UserName = user.Email,
                    Email = user.Email,
                    NormalizedEmail = _userManager.NormalizeEmail(user.Email),
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    PhoneNumber = string.Empty,
                    GoogleRegistration = true,
                };

                var result = await _userManager.CreateAsync(identityUser);

                if (result.Succeeded)
                {

                    return new GenericResponse
                    {
                        StatusCode = 400,
                        Message = "user hasn't been onboarded"
                    };
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogInformation("Error in registration: {data}", error.ToString());
                        Console.WriteLine($"Error: {error.Code} - {error.Description}");
                    }

                    return new GenericResponse
                    {
                        StatusCode = 400,
                        Message = "User registration failed"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user");
                return new GenericResponse
                {
                    StatusCode = 500,
                    Message = "Internal server error",
                };
            }
        }
        public async Task<GenericResponse> ConfirmOtp(ConfirmOtpDTO details)
        {
            var user = await _userManager.FindByEmailAsync(details.Email);
            if (user == null)
            {
                return new GenericResponse
                {
                    StatusCode = 404,
                    Message = "User Not Found"
                };
            }

           
            
            if (user.EmailVerificationCode == null || user.OtpExpiry < DateTime.UtcNow)
            {
                return new GenericResponse
                {
                    StatusCode = 403,
                    Message = "Expired Otp."
                };
            }
            var hashedInputOtp = HashValue(details.Otp);
            if (user.EmailVerificationCode != hashedInputOtp)
            {
                return new GenericResponse
                {
                    StatusCode = 400,
                    Message = "Invalid OTP."
                };
            }

            user.EmailConfirmed = true;
            user.EmailVerificationCode = null; 
            user.OtpExpiry = null;
            await _userManager.UpdateAsync(user);

            return new GenericResponse
            {
                StatusCode = 200,
                Message = "successful"
            };
        }


        public async Task<GenericResponse> RefreshToken(RefreshTokenModel model)
        {
            try
            {
                var principal = _jwtService.GetTokenPrincipal(model.AccessToken);
                var response = new RefreshResponse();

                if (principal?.Identity?.Name is null)
                {
                    return new GenericResponse
                    {
                        StatusCode = 403,
                        Message = "Unauthorized operation"
                    };
                }
                var email = principal.Identity.Name;
                var identityUser = await _userManager.FindByEmailAsync(email);
                var storedHashedToken = await _userManager.GetAuthenticationTokenAsync(identityUser, "Local", "RefreshToken");

                if (identityUser is null || storedHashedToken is null || storedHashedToken != HashValue(model.RefreshToken))
                {
                    return new GenericResponse
                    {
                        StatusCode = 401,
                        Message = "Invalid token"
                    };
                }
                var roles = await _userManager.GetRolesAsync(identityUser);


                var accessToken = JwtService.GenerateAccessTokenAsync(identityUser.Email, identityUser.Id, roles);
                var refreshToken = JwtService.GenerateRefreshToken(identityUser.Email);
                identityUser.RefreshToken = refreshToken;

                await _userManager.UpdateAsync(identityUser);
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

        public async Task<GenericResponse> ForgotPassword(ForgotPasswordDTO user)
        {
            try
            {
                
                var identityuser = await _userManager.FindByEmailAsync(user.Email);

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
    }
}