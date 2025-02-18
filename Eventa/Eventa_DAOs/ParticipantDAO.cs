using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class ParticipantDAO : BaseDAO<Participant>
    {
        public ParticipantDAO(IMongoDatabase database) : base(database, "Participants") { }

        public async Task<Participant?> GetByAccountIdAsync(Guid accountId)
        {
            return await _collection.Find(p => p.AccountId == accountId).FirstOrDefaultAsync();
        }

        public async Task<List<Participant>> GetByEventIdAsync(Guid eventId)
        {
            return await _collection.Find(p => p.EventId == eventId).ToListAsync();
        }
    }
}
