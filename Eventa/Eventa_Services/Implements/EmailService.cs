using Eventa_BusinessObject;
using Eventa_Services.Interfaces;
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

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendVerificationEmail(string email, string verifyUrl)
        {
            try
            {
                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.Username),
                        Subject = "🔒 Xác nhận đăng ký tài khoản Eventa",
                        Body = GetEmailTemplate(verifyUrl),
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi gửi email: {ex.Message}");
            }
        }
        private string GetEmailTemplate(string verifyUrl)
        {
            return $@"
            <html>
            <head>
                <style>
                    body {{
                       font-family: Arial, sans-serif;
                       background-color: #0b0707;
                       text-align: center;
                       
                       height: 100vh; 
                       margin: 0; 
                    }}
                    .container {{
                         max-width: 600px;
                         margin: 20px auto;
                         background: #1c1818;
                         padding: 20px;
                         border-radius: 10px;
                         box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                         margin: 0 auto;
                        
                    }}
                    .header {{
                        background: #ffffff;
                        padding: 15px;
                        color: rgb(12, 11, 11);
                        font-size: 24px;
                        font-weight: bold;
                        border-radius: 5px;
                    }}
                    .content {{
                        padding: 20px;
                        font-size: 14px;
                        font-weight: 700;
                        color: #dcd9d9;
                        display: flex;
flex-direction: column;
                        justify-content: center;
                       align-items: center;
                    }}
                    .verify-button {{
                        display: inline-flex;
                        justify-content: center;
                        padding: 12px 20px;
                        background: #ffffff;
                        color: rgb(9, 8, 8);
                        text-decoration: none;
                        font-weight: bold;
                        border-radius: 5px;
                        margin: 10px 0;
                    }}
                    .footer {{
                        margin-top: 20px;
                        font-size: 14px;
                        color: #585757;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>🎉 Eventa - Xác nhận tài khoản</div>
                    <div class='content'>
                        <p>Chào mừng bạn đến với <b>Eventa</b>!</p>
                        <p>Nhấn vào nút bên dưới để xác nhận tài khoản của bạn.</p>
                        <a href='{verifyUrl}' class='verify-button'>🔐 Xác nhận ngay</a>
                        <p>Nếu bạn không yêu cầu đăng ký tài khoản, hãy bỏ qua email này.</p>
                    </div>
                    <div class='footer'>
                        © 2025 Eventa Corporation. All Rights Reserved.
                    </div>
                </div>
            </body>
            </html>";
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
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
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(to);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi gửi email: {ex.Message}");
            }
        }
    }
}
