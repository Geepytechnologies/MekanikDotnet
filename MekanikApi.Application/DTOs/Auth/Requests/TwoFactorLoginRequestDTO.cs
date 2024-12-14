using System.ComponentModel.DataAnnotations;

namespace MekanikApi.Application.DTOs.Auth.Requests
{
    public class TwoFactorLoginRequestDTO
    {
        [Required]
        public string PinId { get; set; }

        [Required]
        public string Otp { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [Required]
        public string PushToken { get; set; }

    }
}