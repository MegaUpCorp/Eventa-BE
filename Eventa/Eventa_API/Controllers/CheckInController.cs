using Eventa_Services.Interfaces;
using Eventa_BusinessObject.DTOs.Ticket;
using Eventa_BusinessObject.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eventa_BusinessObject.DTOs.CheckIn;

namespace Eventa_Api.Controllers
{
    [ApiController]
    [Route("api/check-in")]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckInParticipant([FromBody] CheckInRequest request)
        {
            try
            {
                var success = await _checkInService.CheckInParticipant(request.ParticipantId, request.EventId);
                return Ok(new { Message = "Check-in thành công" });
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
    }
}