using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Auth.Requests
{
    public class GoogleRequestDTO
    {
        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Middlename { get; set; }

        public string? Email { get; set; }
    }
}
