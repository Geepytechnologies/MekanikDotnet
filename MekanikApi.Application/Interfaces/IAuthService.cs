using MekanikApi.Application.DTOs.Auth.Requests;
using MekanikApi.Application.DTOs.Auth.Responses;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Sms;
using MekanikApi.Domain.Entities;

namespace MekanikApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<GenericResponse> Login(LoginRequestDTO user);
        Task<GenericResponse> RegisterUser(RegisterRequestDTO user);

        Task<GenericResponse> GoogleAuth(GoogleRequestDTO user);

        Task<GenericResponse> ConfirmOtp(ConfirmOtpDTO details);
        Task<bool> SendVerificationCode(string email, string otp, string firstname);

        Task<GenericResponse> RefreshToken(RefreshTokenModel model);

        Task<GenericResponse> ForgotPassword(ForgotPasswordDTO user);

        Task<GenericResponse> ResetPassword(ResetPasswordDTO user, string accessToken);

    }
}