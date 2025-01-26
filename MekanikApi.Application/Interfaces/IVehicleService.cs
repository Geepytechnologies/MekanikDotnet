using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.Interfaces
{
    public interface IVehicleService
    {
        Task<GenericResponse> GetVehicleSpecializations();


        Task<GenericResponse> AddVehicle(VehicleDTO details, string accessToken);
    }
}
