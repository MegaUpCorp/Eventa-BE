using Eventa_BusinessObject.DTOs.Ticket;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost("issue")]
        public async Task<IActionResult> IssueTicket([FromBody] IssueTicketDTO ticketDTO)
        {
            var ticketId = await _ticketService.IssueTicket(ticketDTO);
            return Ok(new { TicketId = ticketId });
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetTicketsByEvent(Guid eventId)
        {
            var tickets = await _ticketService.GetTicketsByEventId(eventId);
            return Ok(tickets);
        }

        [HttpGet("participant/{participantId}")]
        public async Task<IActionResult> GetTicketsByParticipant(Guid participantId)
        {
            var tickets = await _ticketService.GetTicketsByParticipantId(participantId);
            return Ok(tickets);
        }

        [HttpPut("mark-used/{ticketId}")]
        public async Task<IActionResult> MarkTicketAsUsed(Guid ticketId)
        {
            var result = await _ticketService.MarkTicketAsUsed(ticketId);
            if (result)
                return Ok("Ticket marked as used.");
            return BadRequest("Failed to mark ticket as used.");
        }

        [HttpDelete("remove/{ticketId}")]
        public async Task<IActionResult> RemoveTicket(Guid ticketId)
        {
            var result = await _ticketService.RemoveTicket(ticketId);
            if (result)
                return Ok("Ticket removed successfully.");
            return BadRequest("Failed to remove ticket.");
        }
    }
}
