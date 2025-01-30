using MekanikApi.Domain.Entities;
using MekanikApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Mechanic
{
    public sealed record MechanicResponseDTO(Guid Id, Guid UserId, string Name, string Address, int Experience, int CarsFixed, int ResponseTime, DayOfWeek[] WorkDays, int StartHour, string StartMeridien, int EndHour, string EndMeridien, MechanicUserType UserType, VerificationStatus VerificationStatus, string Image, double Latitude, double Longitude, string[] VehicleSpecialization, string[] ServiceSpecialization);

}
