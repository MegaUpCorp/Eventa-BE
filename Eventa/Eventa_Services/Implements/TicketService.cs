using Eventa_BusinessObject.DTOs.Ticket;
using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<string> IssueTicket(IssueTicketDTO ticketDTO)
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                EventId = ticketDTO.EventId,
                ParticipantId = ticketDTO.ParticipantId,
                TicketType = ticketDTO.TicketType,
                Price = ticketDTO.Price,
                IsUsed = false,
                IssuedAt = DateTime.UtcNow
            };

            bool success = await _ticketRepository.AddAsync(ticket);
            return success ? "Ticket issued successfully" : "Failed to issue ticket";
        }

        public async Task<List<Ticket>> GetTicketsByEventId(Guid eventId)
        {
            return await _ticketRepository.GetTicketsByEventIdAsync(eventId);
        }

        public async Task<List<Ticket>> GetTicketsByParticipantId(Guid participantId)
        {
            return await _ticketRepository.GetTicketsByParticipantIdAsync(participantId);
        }
        public async Task<List<Ticket>> GetAllTickets()
        {
            return await _ticketRepository.GetAllTicketsAsync();
        }
        public async Task<bool> MarkTicketAsUsed(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetAsync(ticketId);
            if (ticket == null) return false;

            ticket.IsUsed = true;
            return await _ticketRepository.UpdateAsync(ticket);
        }

        public async Task<bool> RemoveTicket(Guid ticketId)
        {
            return await _ticketRepository.DeleteAsync(ticketId);
        }
    }
}
