using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class CalendarDAO : BaseDAO<Calendar>
    {
        public CalendarDAO(IMongoDatabase database) : base(database, "Calendars") { }

        public async Task<List<Calendar>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(c => c.AccountId == accountId).ToListAsync(cancellationToken);
        }
        public async Task<bool> AddAsync(Calendar calendar, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(calendar, cancellationToken: cancellationToken);
            return true;
        }
    }
}
