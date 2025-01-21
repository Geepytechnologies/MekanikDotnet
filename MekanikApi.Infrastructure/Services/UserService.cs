using MekanikApi.Application.DTOs.Auth.Responses;
using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.Interfaces;
using MekanikApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.Services
{
    public class UserService : IUserService
    {
        public GenericBoolResponse CheckIfCredentialsMatch(ApplicationUser userDetails, string phone, string firstname, string lastname)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse> DeleteUserProfile(string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public Task<GenericTypeResponse<GetUserResponseDTO>> GetUser(string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<GenericTypeResponse<UserLoginResponseDTO>> GetUserById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<UserLoginResponseDTO> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse> UpdateUser(string accessToken, object details)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
