namespace MekanikApi.Application.DTOs.Sms
{
    public class SmsResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object? Result { get; set; }

        public string? pinId { get; set; }

        public string? verified { get; set; }

        public string? msisdn { get; set; }
    }
}