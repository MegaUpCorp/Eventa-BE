using Eventa_BusinessObject.DTOs.Ticket;
using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface ITicketService
    {
        Task<string> IssueTicket(IssueTicketDTO ticketDTO);
        Task<List<Ticket>> GetTicketsByEventId(Guid eventId);
        Task<List<Ticket>> GetAllTickets();
        Task<List<Ticket>> GetTicketsByParticipantId(Guid participantId);
        Task<bool> MarkTicketAsUsed(Guid ticketId);
        Task<bool> RemoveTicket(Guid ticketId);
    }
}
