namespace MekanikApi.Application.DTOs.Sms
{
    public class SmsResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public object? Result { get; set; }

        public string? PinId { get; set; }

        public string? Verified { get; set; }

        public string? Msisdn { get; set; }
    }
}