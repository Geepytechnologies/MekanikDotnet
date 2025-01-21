namespace MekanikApi.Application.DTOs.Sms
{
    public class OtpData
    {
        public string? ApiKey { get; set; }
        public string? MessageType { get; set; }
        public string? To { get; set; }
        public string? From { get; set; }
        public string? Channel { get; set; }
        public int PinAttempts { get; set; }
        public int PinTimeToLive { get; set; }
        public int PinLength { get; set; }
        public string? PinPlaceholder { get; set; }
        public string? MessageText { get; set; }
        public string? PinType { get; set; }
    }
}