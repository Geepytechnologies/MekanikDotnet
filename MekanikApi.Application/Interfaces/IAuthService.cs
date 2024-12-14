using MekanikApi.Application.DTOs.Auth.Requests;
using MekanikApi.Application.DTOs.Auth.Responses;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Domain.Entities;

namespace MekanikApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<GenericResponse> RefreshToken(RefreshTokenModel model);

        Task<LoginResponse> Login(LoginRequestDTO user);

        Task<GenericResponse> ForgotPassword(ForgotPasswordDTO user);

        Task<GenericResponse> ResetPassword(ResetPasswordDTO user, string accessToken);

        Task<RegisterResponse> RegisterUser(RegisterRequestDTO user);

        Task UpdateUser(User user);

        Task<User> GetUser(string email);
    }
}