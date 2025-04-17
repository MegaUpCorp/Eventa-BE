using Eventa_BusinessObject.DTOs.Ticket;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketController> _logger;

        public TicketController(ITicketService ticketService, ILogger<TicketController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpPost("issue")]
        public async Task<IActionResult> IssueTicket([FromBody] IssueTicketDTO ticketDTO)
        {
            try
            {
                var qrCodeUrl = await _ticketService.IssueTicket(ticketDTO);
                return Ok(new { success = true, message = "Ticket issued successfully", qrCodeUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error issuing ticket.");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTickets()
        {
            try
            {
                var tickets = await _ticketService.GetAllTickets();
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tickets.");
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }
        [HttpGet("get/{eventId}")]
        public async Task<IActionResult> GetTicketsByEvent(Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Event ID provided.");
                return BadRequest("Event ID không hợp lệ.");
            }

            try
            {
                var tickets = await _ticketService.GetTicketsByEventId(eventId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tickets for event ID: {EventId}", eventId);
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }

        [HttpGet("get/{participantId}")]
        public async Task<IActionResult> GetTicketsByParticipant(Guid participantId)
        {
            if (participantId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Participant ID provided.");
                return BadRequest("Participant ID không hợp lệ.");
            }

            try
            {
                var tickets = await _ticketService.GetTicketsByParticipantId(participantId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tickets for participant ID: {ParticipantId}", participantId);
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }

        [HttpPut("mark-used/{ticketId}")]
        public async Task<IActionResult> MarkTicketAsUsed(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Ticket ID provided.");
                return BadRequest("Ticket ID không hợp lệ.");
            }

            try
            {
                var result = await _ticketService.MarkTicketAsUsed(ticketId);
                if (result)
                    return Ok("Ticket marked as used.");
                return BadRequest("Failed to mark ticket as used.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking ticket as used for ID: {TicketId}", ticketId);
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }

        [HttpDelete("remove/{ticketId}")]
        public async Task<IActionResult> RemoveTicket(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Ticket ID provided.");
                return BadRequest("Ticket ID không hợp lệ.");
            }

            try
            {
                var result = await _ticketService.RemoveTicket(ticketId);
                if (result)
                    return Ok("Ticket removed successfully.");
                return BadRequest("Failed to remove ticket.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing ticket with ID: {TicketId}", ticketId);
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }
    }
}
