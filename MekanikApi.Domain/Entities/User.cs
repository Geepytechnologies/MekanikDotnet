using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Entities
{
    public class User: IdentityUser<Guid>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? Middlename { get; set; }

        [StringLength(11, ErrorMessage = "Phone must be 11 characters long.")]
        public override string PhoneNumber { get; set; }

        public string? PushToken { get; set; }


        public bool IsVerified { get; set; } = false;

        public string? Latitude { get; set; }

        public string? Longitude { get; set; }

        public ICollection<Conversation> Conversations { get; set; }
    }
}
