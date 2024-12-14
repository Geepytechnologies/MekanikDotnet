﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace MekanikApi.Infrastructure.Services
{
    public class EmailService
    {
        private readonly IConfiguration _emailConfig;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration emailConfig, ILogger<EmailService> logger)
        {
            _emailConfig = emailConfig;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject)
        {
            try
            {
                var body = await GetEmailBodyAsync("template.html");

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_emailConfig.GetSection("EmailConfiguration:From").Value));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = body
                };

                using var smtp = new SmtpClient();
                smtp.Connect(_emailConfig.GetSection("EmailConfiguration:SmtpServer").Value, 465, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate(_emailConfig.GetSection("EmailConfiguration:Username").Value, _emailConfig.GetSection("EmailConfiguration:Password").Value);
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