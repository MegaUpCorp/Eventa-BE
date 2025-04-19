using Appwrite;
using Eventa_BusinessObject.DTOs.Ticket;
using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Implements;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using Eventa_Services.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IParticipantRepository _participantRepository;
        private readonly ILogger<TicketService> _logger;
        private readonly IConfiguration _configuration;

        public TicketService(
            ITicketRepository ticketRepository,
            IParticipantRepository participantRepository,
            ILogger<TicketService> logger,
            IConfiguration configuration)
        {
            _ticketRepository = ticketRepository;
            _participantRepository = participantRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> IssueTicket(IssueTicketDTO ticketDTO)
        {
            var participant = await _participantRepository.GetAsync(ticketDTO.ParticipantId);
            if (participant == null)
            {
                _logger.LogError("Participant not found with ID: {ParticipantId}", ticketDTO.ParticipantId);
                return "Participant not found";
            }
            var ticket = new Ticket
            {
                EventId = ticketDTO.EventId,
                ParticipantId = ticketDTO.ParticipantId,
                TicketType = ticketDTO.TicketType,
                Price = ticketDTO.Price,
                IsUsed = false,
                IssuedAt = DateTime.UtcNow
            };
            bool success = await _ticketRepository.AddAsync(ticket);
            if (!success)
            {
                _logger.LogError("Failed to issue ticket for Participant ID: {ParticipantId}", ticketDTO.ParticipantId);
                return "Failed to issue ticket";
            }
            return $"Ticket ID: {ticket.Id} đã được phát hành cho người tham gia {participant.AccountId} với loại vé {ticket.TicketType}.";
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
