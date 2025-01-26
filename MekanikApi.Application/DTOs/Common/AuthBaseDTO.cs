using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Common
{
    public abstract class AuthBaseDTO
    {
        public string AccessToken { get; set; }
    }
}
