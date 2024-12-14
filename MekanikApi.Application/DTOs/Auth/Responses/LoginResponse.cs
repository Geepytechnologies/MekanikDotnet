namespace MekanikApi.Application.DTOs.Auth.Responses
{
    public class LoginResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public UserLoginResponseDTO? UserDetails { get; set; }
        public UserNotVerifiedResponseDTO? UserNotVerifiedDetails { get; set; }

        public object? Otp2FAResponse { get; set; }
    }
}