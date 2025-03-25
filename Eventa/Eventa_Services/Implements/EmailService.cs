using Eventa_BusinessObject;
using Eventa_Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendVerificationEmail(string email, string verifyUrl)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(verifyUrl))
            {
                _logger.LogWarning("Email hoặc Verify URL không hợp lệ.");
                return false;
            }

            try
            {
                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.Username),
                        Subject = "Verify your email",
                        Body = $"Click here to verify your account: <a href='{verifyUrl}'>Verify</a>",
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email xác minh đã gửi tới {Email}", email);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email xác minh tới {Email}", email);
                return false;
            }
        }
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                Console.WriteLine("Thông tin email không hợp lệ.");
                return false;
            }

            try
            {
                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.Username),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true, // Đảm bảo email là HTML
                    };
                    mailMessage.To.Add(to);

                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine($"Email đã gửi tới {to} với subject {subject}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email tới {to}: {ex.Message}");
                return false;
            }
        }
    }
}
