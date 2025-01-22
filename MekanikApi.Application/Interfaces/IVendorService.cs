using MekanikApi.Application.DTOs.Common;
using MekanikApi.Application.DTOs.Mechanic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.Interfaces
{
    public interface IVendorService
    {
        Task<GenericResponse> CreateVendorProfile(CreateMechanicDTO details, string accessToken);

        Task<GenericResponse> GetAllVendors();

        Task<GenericResponse> GetVendorProfile(Guid vendorId);

        Task<GenericResponse> FindVendorsNearMe(double latitude, double longitude);
    }
}
