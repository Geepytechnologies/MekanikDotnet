using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace MekanikApi.Infrastructure.Services
{
    public class EmailService(IConfiguration emailConfig, ILogger<EmailService> logger): IEmailService
    {
        private readonly IConfiguration _emailConfig = emailConfig;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                //var body = await GetEmailBodyAsync("verification.html");

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("MAIL_ACCOUNT")));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = body
                };

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Connect(Environment.GetEnvironmentVariable("MAIL_SERVER"), 465, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate(Environment.GetEnvironmentVariable("MAIL_USERNAME"), Environment.GetEnvironmentVariable("MAIL_PASSWORD"));
                smtp.Send(email);
                smtp.Disconnect(true);

                return true;
            }
            catch (SmtpCommandException ex)
            {
                // This exception is thrown when the SMTP server returns an error in response to a command.
                Console.WriteLine($"SMTP command error: {ex.Message}");
            }
            catch (SmtpProtocolException ex)
            {
                // This exception is thrown when there is an error in the underlying SMTP protocol.
                Console.WriteLine($"SMTP protocol error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error sending email: {ex}");
            }

            // The email sending failed
            return false;
        }

        public static async Task<string> GetEmailBodyAsync(string templateFileName)
        {
            var templatePath = Path.Combine("EmailTemplates", templateFileName);

            if (File.Exists(templatePath))
            {
                return await File.ReadAllTextAsync(templatePath);
            }

            throw new FileNotFoundException($"Email template file '{templateFileName}' not found.");
        }
    }
}

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string body);
}