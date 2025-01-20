using MekanikApi.Domain.Enums;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public enum VerificationStatus
    {
        NotVerified = 0,
        Verified = 1,
    }
    public class Mechanic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int Experience { get; set; }

        public int CarsFixed { get; set; }

        public int ResponseTime { get; set; }

        public DayOfWeek WorkDays { get; set; }

        public int StartHour { get; set; }

        public string StartMeridien { get; set; }

        public int EndHour { get; set; }

        public string EndMeridien { get; set; }

        public MechanicUserType UserType { get; set; } = MechanicUserType.Basic;

        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.NotVerified;

        public Point Location { get; set; }

        public IFormFile Image { get; set; }

        public ICollection<Feedback> Feedbacks { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
