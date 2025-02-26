using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class NotificationDAO : BaseDAO<Notification>
    {
        public NotificationDAO(IMongoDatabase database) : base(database, "Notifications") { }

        public async Task<List<Notification>> GetByAccountIdAsync(Guid accountId)
        {
            return await _collection.Find(n => n.AccountId == accountId).ToListAsync();
        }
    }
}
