using MekanikApi.Application.DTOs.Common;
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
    }
}
