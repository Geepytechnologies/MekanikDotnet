using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Sms
{
    public class OtpResult
    {
        public string? Verified { get; set; }
        public string? PinId { get; set; }
        public string? Msisdn { get; set; }
    }
    public class OtpVerificationResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public OtpResult? Result { get; set; }
    }
}
