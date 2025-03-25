using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmail(string email, string verifyUrl);
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
