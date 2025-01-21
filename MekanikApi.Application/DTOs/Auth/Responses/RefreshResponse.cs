namespace MekanikApi.Application.DTOs.Auth.Responses
{
    public class RefreshResponse
    {
        public bool Status { get; set; } = false;

        public string? AccessToken { get; set; }
    }
}