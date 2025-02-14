using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string verifyUrl);
    }
}
