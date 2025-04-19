using Eventa_BusinessObject.DTOs.Participant;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers
{
    [ApiController]
    [Route("api/participants")]
    [Authorize]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantService _participantService;
        private readonly IEventService _eventService;
        private readonly ILogger<ParticipantController> _logger;

        public ParticipantController(IParticipantService participantService, IEventService eventService, ILogger<ParticipantController> logger)
        {
            _participantService = participantService;
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet("get/{eventId}")]
        public async Task<IActionResult> GetParticipantsByEvent(Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Event ID provided.");
                return BadRequest("Event ID không hợp lệ.");
            }

            try
            {
                var eventItem = await _eventService.GetEventById(eventId);
                if (eventItem == null)
                {
                    _logger.LogWarning("Event not found for ID: {EventId}", eventId);
                    return NotFound("Sự kiện không tồn tại.");
                }

                var participants = await _participantService.GetParticipantsByEventId(eventId);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving participants for event ID: {EventId}", eventId);
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterParticipant([FromBody] RegisterParticipantRequest request)
        {
            try
            {
                var accountIdClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
                {
                    return Unauthorized("Không thể xác định tài khoản từ token.");
                }

                if (request.EventId == Guid.Empty)
                    return BadRequest("Event ID không hợp lệ.");

                var result = await _participantService.RegisterParticipant(accountId, request.EventId);
                return Ok(new { success = true, message = "Đăng ký thành công." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error during participant registration.");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering participant.");
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpDelete("remove/{participantId}")]
        public async Task<IActionResult> RemoveParticipant(Guid participantId)
        {
            if (participantId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Participant ID provided.");
                return BadRequest("Participant ID không hợp lệ.");
            }

            try
            {
                var result = await _participantService.RemoveParticipant(participantId);
                return Ok(new { Message = "Xóa participant thành công." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error during participant removal.");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing participant with ID: {ParticipantId}", participantId);
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }
        [HttpGet("get-all-participant-of-event")]
        public async Task<IActionResult> GetParticipantsOfEvent(string slug)
        {
            try
            {
                var participants = await _participantService.GetParticipantsOfEvent(slug);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving participants for event slug: {Slug}", slug);
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }
        [HttpGet("event-participated-me")]
        public async Task<IActionResult> GetAllEventParticipantedOfMe()
        {
            try
            {
                var eventIds = await _participantService.GetAllEventParticipantedOfMe(HttpContext);
                return Ok(eventIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all events participated by user.");
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }
    }
}
