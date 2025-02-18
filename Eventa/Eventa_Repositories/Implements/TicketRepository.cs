using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public Task<int> CountAsync(Expression<Func<Ticket, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket?> GetAsync(Expression<Func<Ticket, bool>> filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAllAsync(Expression<Func<Ticket, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<Ticket> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Ticket entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRange(IEnumerable<Ticket> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(params Ticket[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
