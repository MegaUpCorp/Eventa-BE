using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class OrganizerDAO : BaseDAO<Organizer>
    {
        public OrganizerDAO(IMongoDatabase database) : base(database, "Organizers") { }

        public async Task<Organizer?> GetByAccountIdAsync(Guid accountId)
        {
            return await _collection.Find(o => o.AccountId == accountId).FirstOrDefaultAsync();
        }
    }
}
