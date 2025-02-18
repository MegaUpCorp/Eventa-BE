using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class CheckInDAO : BaseDAO<CheckIn>
    {
        public CheckInDAO(IMongoDatabase database) : base(database, "CheckIns") { }

        public async Task<CheckIn?> GetByParticipantAndEventAsync(Guid participantId, Guid eventId)
        {
            return await _collection.Find(c => c.ParticipantId == participantId && c.EventId == eventId)
                                    .FirstOrDefaultAsync();
        }
    }
}
