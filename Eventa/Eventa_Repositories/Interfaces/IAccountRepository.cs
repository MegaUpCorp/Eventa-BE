using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account?> GetAccountByUsernameAsync(string username);
        Task<Account?> GetAccountByPhoneNumberAsync(string phoneNumber);
    }
}
