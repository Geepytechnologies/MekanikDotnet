using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
using MekanikApi.Application.DTOs.Sms;
using MekanikApi.Application.Interfaces;
using MekanikApi.Infrastructure.serializers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MekanikApi.Infrastructure.Services
{
    public class OtpSendResponse
    {
        public string pinId { get; set; }
        public string to { get; set; }
        public string smsStatus { get; set; }
        public string? message { get; set; }
    }

    
    public class SmsService : ISmsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<SmsService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<SmsResponse> SendForgotAccountPinOtp(string phone)
        {
            var httpClient = _httpClientFactory.CreateClient("termii");
            var apiKey = Environment.GetEnvironmentVariable("Termiikey");

            var data = new
            {
                api_key = apiKey,
                message_type = "ALPHANUMERIC",
                to = phone,
                from = "N-Alert",
                channel = "dnd",
                pin_attempts = 10,
                pin_time_to_live = 5,
                pin_length = 6,
                pin_placeholder = "<>",
                message_text = $"DO NOT DISCLOSE. To reset Payment PIN, please use OTP <>. No staff of SisPay will ask for this code.",
                pin_type = "NUMERIC"
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("api/sms/otp/send", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<OtpSendResponse>(responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("An error occurred while sending OTP: {StatusCode}", response.StatusCode);
                    return new SmsResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "An error occurred while sending OTP.",
                        Result = apiResponse
                    };
                }
                else
                {
                    return new SmsResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "OTP sent successfully.",
                        Result = apiResponse
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending OTP.");
                return new SmsResponse
                {
                    StatusCode = 500,
                    Message = "Internal server error."
                };
            }
        }

        public async Task<SmsResponse> SendOtp(OtpRequestDTO request)
        {
            var httpClient = _httpClientFactory.CreateClient("termii");
            var apiKey = Environment.GetEnvironmentVariable("Termiikey");

            var data = new
            {
                api_key = apiKey,
                message_type = "ALPHANUMERIC",
                to = request.MobileNumber,
                from = "N-Alert",
                channel = "dnd",
                pin_attempts = 10,
                pin_time_to_live = 5,
                pin_length = 6,
                pin_placeholder = "<>",
                message_text = $"Hi {request.Name}, Your Sispay authentication pin is <>. If you did not initiate this request, Please Ignore. Do not share this code with anyone.",
                pin_type = "NUMERIC"
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("api/sms/otp/send", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<OtpSendResponse>(responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("An error occurred while sending OTP: {StatusCode}", response.StatusCode);
                    return new SmsResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "An error occurred while sending OTP.",
                        Result = apiResponse
                    };
                }
                else
                {
                    _logger.LogInformation("OTP fail details: {msg}", Newtonsoft.Json.JsonConvert.SerializeObject(apiResponse));
                    return new SmsResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "OTP sent successfully.",
                        Result = apiResponse
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending OTP.");
                return new SmsResponse
                {
                    StatusCode = 500,
                    Message = "Internal server error."
                };
            }
        }

        public async Task<SmsResponse> SendForgotPasswordOtp(ForgotPasswordOtpRequestDTO request)
        {
            var httpClient = _httpClientFactory.CreateClient("termii");
            var apiKey = Environment.GetEnvironmentVariable("Termiikey");

            var data = new
            {
                api_key = apiKey,
                message_type = "ALPHANUMERIC",
                to = request.MobileNumber,
                from = "N-Alert",
                channel = "dnd",
                pin_attempts = 10,
                pin_time_to_live = 5,
                pin_length = 6,
                pin_placeholder = "<>",
                message_text = $"Your Sispay authentication pin is <>. If you did not initiate this request, Please Ignore. Do not share this code with anyone.",
                pin_type = "NUMERIC"
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("api/sms/otp/send", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<OtpSendResponse>(responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("An error occurred while sending OTP: {StatusCode}", response.StatusCode);
                    return new SmsResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "An error occurred while sending OTP.",
                        Result = apiResponse
                    };
                }
                else
                {
                    return new SmsResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "OTP sent successfully.",
                        Result = apiResponse
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending OTP.");
                return new SmsResponse
                {
                    StatusCode = 500,
                    Message = "Internal server error."
                };
            }
        }

        

       
    }
}