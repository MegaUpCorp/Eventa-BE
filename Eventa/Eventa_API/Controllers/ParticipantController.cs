using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers
{
    [ApiController]
    [Route("api/participants")]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public ParticipantController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [HttpGet("get/{eventId}")]
        public async Task<IActionResult> GetParticipantsByEvent(Guid eventId)
        {
            var participants = await _participantService.GetParticipantsByEventId(eventId);
            return Ok(participants);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterParticipant(Guid accountId, Guid eventId)
        {
            var result = await _participantService.RegisterParticipant(accountId, eventId);
            if (result)
                return Ok("Registration successful.");
            return BadRequest("Registration failed.");
        }

        [HttpDelete("remove/{participantId}")]
        public async Task<IActionResult> RemoveParticipant(Guid participantId)
        {
            var result = await _participantService.RemoveParticipant(participantId);
            if (result)
                return Ok("Participant removed successfully.");
            return BadRequest("Failed to remove participant.");
        }
        [HttpGet("get-all-participant-of-event")]
        public async Task<IActionResult> GetAllParticipantOfEvent(string slug)
        {
            var participants = await _participantService.GetParticipantsOfEvent(slug);
            return Ok(participants);
        }
    }

}
