using Eventa_BusinessObject.DTOs.Account;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IAccountService
    {
        public Task<bool> Register(CreateAccountRequest createAccountRequest);
        
    }
}
