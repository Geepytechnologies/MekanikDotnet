using MekanikApi.Application.DTOs.Sms;

namespace MekanikApi.Application.Interfaces
{
    public interface ISmsService
    {
        Task<SmsResponse> SendOtp(OtpRequestDTO request);
        Task<SmsResponse> SendForgotPasswordOtp(ForgotPasswordOtpRequestDTO request);

        Task<SmsResponse> SendForgotAccountPinOtp(string phone);

        

    }
}