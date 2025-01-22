using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Mechanic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.Interfaces
{
    public interface IMechanicService
    {
        Task<GenericResponse> CreateMechanicProfile(CreateMechanicDTO details, string accessToken);

        Task<GenericResponse> GetAllMechanics();

        Task<GenericResponse> GetMechanicProfile(Guid mechanicId);

        Task<GenericResponse> FindMechanicsNearMe(double latitude, double longitude);
    }
}
