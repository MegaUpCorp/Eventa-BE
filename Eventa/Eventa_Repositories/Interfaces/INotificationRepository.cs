using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<List<Notification>> GetByAccountIdAsync(Guid accountId);
    }
}
