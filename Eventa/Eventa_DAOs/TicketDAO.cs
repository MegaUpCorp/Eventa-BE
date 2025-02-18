using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class TicketDAO : BaseDAO<Ticket>
    {
        public TicketDAO(IMongoDatabase database) : base(database, "Tickets") { }

        public async Task<List<Ticket>> GetTicketsByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(t => t.EventId == eventId).ToListAsync(cancellationToken);
        }

        public async Task<List<Ticket>> GetTicketsByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(t => t.ParticipantId == participantId).ToListAsync(cancellationToken);
        }
    }
}
