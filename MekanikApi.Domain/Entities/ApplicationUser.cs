using MekanikApi.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public string? Middlename { get; set; }

        [StringLength(11, ErrorMessage = "Phone must be 11 characters long.")]
        public new string? PhoneNumber { get; set; }

        public string? PushToken { get; set; }

        public string? EmailVerificationCode { get; set; }
        public DateTime? OtpExpiry { get; set; }

        public Point? Location { get; set; }

        public ICollection<Conversation>? Conversations { get; set; } = [];

        public ICollection<Conversation>? ParticipantConversations { get; set; } = [];

        // Feedback relationships
        public ICollection<Feedback> FeedbacksSent { get; set; } = [];
        public ICollection<Feedback> FeedbacksReceived { get; set; } = [];


        public ApplicationProfile[]? Profile { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }

        public bool? GoogleRegistration { get; set; }

        public bool? FacebookRegistration { get; set; }


    }
}
