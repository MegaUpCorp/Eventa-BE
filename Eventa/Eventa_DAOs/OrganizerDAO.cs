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
        private readonly IMongoDatabase _database;

        public OrganizerDAO(IMongoDatabase database) : base(database, "Organizers")
        {
            _database = database;
        }

        public async Task<Organizer?> GetByAccountIdAsync(Guid accountId)
        {
            return await _collection.Find(o => o.AccountId == accountId).FirstOrDefaultAsync();
        }
        public async Task<Account?> GetAccountByOrganizerId(Guid organizerId)
        {
            var organizer = await _collection.Find(o => o.Id == organizerId).FirstOrDefaultAsync();
            if (organizer == null)
                return null;
            return await _database.GetCollection<Account>("Accounts").Find(a => a.Id == organizer.AccountId).FirstOrDefaultAsync();
        }
        public async Task<List<Guid>> GetOrganizerIdsByEventId(Guid eventId)
        {
            var eventWithOrganizers = await _database.GetCollection<Event>("Event")
                                                     .Find(e => e.Id == eventId)
                                                     .FirstOrDefaultAsync();
            return eventWithOrganizers?.OrganizerId ?? new List<Guid>();
        }
        public async Task<bool> CheckAccountInOrganizers(Guid accountId, List<Guid> organizerIds)
        {
            return await Task.FromResult(organizerIds.Contains(accountId));
        }
    }
}
