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

        public async Task<OtpVerificationResponse> VerifyOTP(ConfirmOtpDTO request)
        {
            var httpClient = _httpClientFactory.CreateClient("termii");
            var apiKey = Environment.GetEnvironmentVariable("Termiikey");

            var data = new
            {
                api_key = apiKey,
                pin_id = request.PinId,
                pin = request.Otp,
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("api/sms/otp/verify", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement apiResponse = doc.RootElement;

                    // Extracting values from the response
                    string verified;
                    if (apiResponse.GetProperty("verified").ValueKind == JsonValueKind.True || apiResponse.GetProperty("verified").ValueKind == JsonValueKind.False)
                    {
                        verified = apiResponse.GetProperty("verified").GetBoolean().ToString(); 
                    }
                    else
                    {
                        verified = apiResponse.GetProperty("verified").GetString(); 
                    }// Extract "verified"
                    string pinId = apiResponse.GetProperty("pinId").GetString();      
                    string msisdn = apiResponse.GetProperty("msisdn").GetString();    
                    //int status = apiResponse.GetProperty("status").GetInt32();        

                    // Handling response based on status code
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("An error occurred while verifying OTP: {StatusCode}", response.StatusCode);
                        
                        return new OtpVerificationResponse
                        {
                            
                            StatusCode = (int)response.StatusCode,
                            Message = "An error occurred while verifying OTP.",
                            Result = new OtpResult
                            {
                                Verified = verified.ToString(),
                                Msisdn = msisdn,
                                PinId = pinId
                            }
                        };
                    }
                    else
                    {
                        return new OtpVerificationResponse
                        {
                            StatusCode = 200,
                            Message = response.ReasonPhrase,
                            Result = new OtpResult
                            {
                                Verified = verified.ToString(),
                                Msisdn = msisdn,
                                PinId = pinId
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while verifying OTP.");
                return new OtpVerificationResponse
                {
                    StatusCode = 500,
                    Message = "Internal server error."
                };
            }
        }


        public async Task<SmsResponse> VoiceOTP(VoiceOtpDTO request)
        {
            var httpClient = _httpClientFactory.CreateClient("termii");
            var apiKey = Environment.GetEnvironmentVariable("Termiikey");

            var data = new
            {
                api_key = apiKey,
                phone_number = request.MobileNumber,
                pin_attempts = 10,
                pin_time_to_live = 5,
                pin_length = 6
            };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("api/sms/otp/send/voice", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<OtpSendResponse>(responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("An error occurred while calling OTP: {StatusCode}", response.StatusCode);
                    return new SmsResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "An error occurred while calling OTP.",
                        Result = response
                    };
                }
                else
                {
                    return new SmsResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "OTP called successfully.",
                        Result = responseBody
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while verifying OTP.");
                return new SmsResponse
                {
                    StatusCode = 500,
                    Message = "Internal server error."
                };
            }
        }

       
    }
}