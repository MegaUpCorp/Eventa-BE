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

        public ParticipantController(IParticipantService participantService, IEventService eventService)
        {
            _participantService = participantService;
            _eventService = eventService;
        }

        [HttpGet("get/{eventId}")]
        public async Task<IActionResult> GetParticipantsByEvent(Guid eventId)
        {
            if (eventId == Guid.Empty)
                return BadRequest("Event ID không hợp lệ.");

            var eventItem = await _eventService.GetEventById(eventId);
            if (eventItem == null)
                return NotFound("Sự kiện không tồn tại.");

            var participants = await _participantService.GetParticipantsByEventId(eventId);
            return Ok(participants);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterParticipant([FromBody] RegisterParticipantRequest request)
        {
            try
            {
                // Lấy accountId từ JWT token
                var accountIdClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out Guid accountId))
                {
                    return Unauthorized("Không thể xác định tài khoản từ token.");
                }

                if (request.EventId == Guid.Empty)
                    return BadRequest("Event ID không hợp lệ.");

                var result = await _participantService.RegisterParticipant(accountId, request.EventId);
                return Ok(new { Message = "Đăng ký thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpDelete("remove/{participantId}")]
        public async Task<IActionResult> RemoveParticipant(Guid participantId)
        {
            if (participantId == Guid.Empty)
                return BadRequest("Participant ID không hợp lệ.");

            try
            {
                var result = await _participantService.RemoveParticipant(participantId);
                return Ok(new { Message = "Xóa participant thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("get-all-participant-of-event")]
        public async Task<IActionResult> GetParticipantsOfEvent(string slug)
        {
            var participants = await _participantService.GetParticipantsOfEvent(slug);
            return Ok(participants);
        }

    }
}
