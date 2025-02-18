using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Interfaces
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<List<Ticket>> GetTicketsByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task<List<Ticket>> GetTicketsByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default);
    }
}
