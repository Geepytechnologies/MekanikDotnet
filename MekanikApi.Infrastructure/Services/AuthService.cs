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

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, ILogger<AuthService> logger, ICacheService cacheService, IEmailService emailService, IJwtService jwtService, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _config = config;
            _logger = logger;
            _cacheService = cacheService;
            _emailService = emailService;
            _jwtService = jwtService;
            _signInManager = signInManager;
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
        public static async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string accessToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") }
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
            var result = await _emailService.SendEmailAsync(email, "Clusstr Account Verification", emailbody);
            return result;
        }
        public async Task<GenericResponse> Login(LoginRequestDTO user)
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
                    _applicationUser.RefreshToken = hashedRefreshToken;
                    _applicationUser.RefreshTokenExpiry = DateTime.Now.AddHours(2);

                    await _userManager.UpdateAsync(_applicationUser);




                    var userDetails = new UserLoginResponseDTO(
                        UserId: _applicationUser.Id,
                        FirstName: _applicationUser.Firstname,
                        MiddleName: _applicationUser.Middlename,
                        LastName: _applicationUser.Lastname,
                        PhoneNumber: _applicationUser.PhoneNumber,
                        Email: _applicationUser.Email,
                        Gender: _applicationUser.Gender,
                        AccessToken: tokenstring,
                        RefreshToken: refreshToken
                        )


                    ;
                    if (!_applicationUser.CompletedOnboarding)
                    {
                        return new GenericResponse
                        {
                            StatusCode = 200,
                            Message = "user hasn't been onboarded",
                            Result = userDetails
                        };
                    }
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
                    var expiration = DateTime.UtcNow.AddMinutes(5);

                    var existingClaims = await _userManager.GetClaimsAsync(identityuser);
                    var otpCodeClaim = existingClaims.FirstOrDefault(c => c.Type == "otp_code");
                    var otpExpirationClaim = existingClaims.FirstOrDefault(c => c.Type == "otp_expiration");

                    if (otpCodeClaim != null)
                    {
                        await _userManager.RemoveClaimAsync(identityuser, otpCodeClaim);
                    }
                    if (otpExpirationClaim != null)
                    {
                        await _userManager.RemoveClaimAsync(identityuser, otpExpirationClaim);
                    }

                    var otpClaims = new List<Claim>
                    {
                    new("otp_code", otp),
                    new("otp_expiration", expiration.ToString())
                    };

                    await _userManager.AddClaimsAsync(identityuser, otpClaims);
                    await SendVerificationCode(user.Email, otp, identityuser.Firstname);
                    return new GenericResponse
                    {
                        StatusCode = 401,
                        Message = "User is not verified, Otp has been sent to email",
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
                    NormalizedEmail = _userManager.NormalizeEmail(user.Email),
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                };

                var result = await _userManager.CreateAsync(identityUser, user.Password);

                if (result.Succeeded)
                {
                    var expiration = DateTime.UtcNow.AddMinutes(5);

                    var existingClaims = await _userManager.GetClaimsAsync(identityUser);
                    var otpCodeClaim = existingClaims.FirstOrDefault(c => c.Type == "otp_code");
                    var otpExpirationClaim = existingClaims.FirstOrDefault(c => c.Type == "otp_expiration");

                    if (otpCodeClaim != null)
                    {
                        await _userManager.RemoveClaimAsync(identityUser, otpCodeClaim);
                    }

                    if (otpExpirationClaim != null)
                    {
                        await _userManager.RemoveClaimAsync(identityUser, otpExpirationClaim);
                    }

                    var otpClaims = new List<Claim>
                    {
                        new("otp_code", otp),
                        new("otp_expiration", expiration.ToString())
                    };

                    var addClaimsResult = await _userManager.AddClaimsAsync(identityUser, otpClaims);
                    if (!addClaimsResult.Succeeded)
                    {
                        foreach (var error in addClaimsResult.Errors)
                        {
                            _logger.LogInformation("Error in adding claims: {data}", error.ToString());
                            Console.WriteLine($"Error: {error.Code} - {error.Description}");
                        }

                        return new GenericResponse
                        {
                            StatusCode = 400,
                            Message = "User created, but failed to add claims"
                        };
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
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser != null)
                {

                    var tokenstring = await _jwtService.GenerateAccessTokenAsync(user.Email);
                    var refreshToken = JwtService.GenerateRefreshToken(user.Email);
                    existingUser.RefreshToken = refreshToken;
                    existingUser.RefreshTokenExpiry = DateTime.Now.AddHours(2);
                    await _userManager.UpdateAsync(existingUser);

                    var userDetails = new UserLoginResponseDTO(
                        UserId: existingUser.Id,
                        FirstName: existingUser.Firstname,
                        MiddleName: existingUser.Middlename,
                        LastName: existingUser.Lastname,
                        PhoneNumber: existingUser.PhoneNumber,
                        Email: existingUser.Email,
                        Gender: existingUser.Gender,
                        AccessToken: tokenstring,
                        RefreshToken: refreshToken
                        );

                    if (!existingUser.CompletedOnboarding)
                    {
                        return new GenericResponse
                        {
                            StatusCode = 200,
                            Message = "user hasn't been onboarded",
                            Result = userDetails
                        };
                    }
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
                    Gender = string.Empty
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
                    Message = "Invalid email address."
                };
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var otpCodeClaim = claims.FirstOrDefault(c => c.Type == "otp_code");
            var otpExpirationClaim = claims.FirstOrDefault(c => c.Type == "otp_expiration");

            if (otpCodeClaim == null || otpExpirationClaim == null)
            {
                return new GenericResponse
                {
                    StatusCode = 400,
                    Message = "Invalid OTP code"
                };
            }

            var otpExpiration = DateTime.Parse(otpExpirationClaim.Value);
            if (otpCodeClaim.Value == details.Otp && otpExpiration > DateTime.UtcNow)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                await _userManager.RemoveClaimsAsync(user, new List<Claim> { otpCodeClaim, otpExpirationClaim });
                return new GenericResponse
                {
                    StatusCode = 200,
                    Message = "Email confirmed successfully."
                };
            }
            if (otpCodeClaim.Value == details.Otp && otpExpiration < DateTime.UtcNow)
            {
                await _userManager.RemoveClaimsAsync(user, new List<Claim> { otpCodeClaim, otpExpirationClaim });
                return new GenericResponse
                {
                    StatusCode = 403,
                    Message = "Expired Otp."
                };
            }
            return new GenericResponse
            {
                StatusCode = 400,
                Message = "OTP confirmation not successful"
            };
        }


        public async Task<GenericResponse> RefreshToken(string RefreshToken)
        {
            var principal = _jwtService.GetTokenPrincipal(RefreshToken);

            if (principal is null)
            {
                return new GenericResponse
                {
                    StatusCode = 403,
                    Message = "Error validating token"
                };
            }

            var claimemail = principal?.FindFirst(ClaimTypes.Email)?.Value;

            var identityUser = await _userManager.FindByEmailAsync(claimemail);

            if (identityUser is null)
            {
                return new GenericResponse
                {
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            if (identityUser.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return new GenericResponse
                {
                    StatusCode = 401,
                    Message = "Expired token"
                };
            }

            var email = identityUser.Email;
            LoginRequestDTO user = new LoginRequestDTO { Email = email };
            var accessToken = await _jwtService.GenerateAccessTokenAsync(user.Email);
            return new GenericResponse
            {
                StatusCode = 200,
                Message = "Successful",
                Result = accessToken
            };

        }


    }
}