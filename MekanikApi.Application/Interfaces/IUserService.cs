using MekanikApi.Application.DTOs.Auth.Responses;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Domain.Entities;

namespace MekanikApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<object>> GetAllUsers();

        Task<UserLoginResponseDTO> GetUserByPhoneNumberAsync(string phoneNumber);

        Task<GenericTypeResponse<UserLoginResponseDTO>> GetUserById(Guid id);

        Task<GenericTypeResponse<GetUserResponseDTO>> GetUser(string accessToken);

        Task<GenericResponse> DeleteUserProfile(string accessToken);

        GenericBoolResponse CheckIfCredentialsMatch(User userDetails, string phone, string firstname, string lastname);

        Task<bool> VerifyUserAsync(Guid userId);

        Task<string> DeleteUser(Guid id);

        Task<GenericResponse> UpdateUser(string accessToken, object details);
    }
}