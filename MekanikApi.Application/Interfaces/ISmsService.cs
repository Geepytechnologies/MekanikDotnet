using MekanikApi.Application.DTOs.Sms;

namespace MekanikApi.Application.Interfaces
{
    public interface ISmsService
    {
        Task<SmsResponse> SendOtp(OtpRequestDTO request);
        Task<SmsResponse> SendForgotPasswordOtp(ForgotPasswordOtpRequestDTO request);

        Task<SmsResponse> SendForgotAccountPinOtp(string phone);

        Task<OtpVerificationResponse> VerifyOTP(ConfirmOtpDTO request);

        Task<SmsResponse> VoiceOTP(VoiceOtpDTO request);
    }
}