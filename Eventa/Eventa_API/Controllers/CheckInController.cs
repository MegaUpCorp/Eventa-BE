using Eventa_Services.Interfaces;
using Eventa_BusinessObject.DTOs.Ticket;
using Eventa_BusinessObject.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventa_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn(Guid participantId, Guid eventId)
        {
            var result = await _checkInService.CheckInParticipant(participantId, eventId);
            if (result)
                return Ok("Check-in successful.");
            return BadRequest("Check-in failed.");
        }
    }
}