using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Implements
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketDAO _ticketDAO;

        public TicketRepository(TicketDAO ticketDAO)
        {
            _ticketDAO = ticketDAO;
        }

        public async Task<bool> AddAsync(Ticket entity, CancellationToken cancellationToken = default)
        {
            return await _ticketDAO.AddAsync(entity, cancellationToken);
        }

        public async Task<List<Ticket>> GetTicketsByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await _ticketDAO.GetTicketsByEventIdAsync(eventId, cancellationToken);
        }

        public async Task<List<Ticket>> GetTicketsByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default)
        {
            return await _ticketDAO.GetTicketsByParticipantIdAsync(participantId, cancellationToken);
        }

        public async Task<bool> UpdateAsync(Ticket ticket)
        {
            return await _ticketDAO.UpdateAsync(ticket);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _ticketDAO.DeleteAsync(id);
        }
    }
}
