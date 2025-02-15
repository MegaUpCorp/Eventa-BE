using Eventa_BusinessObject.Entities;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Eventa_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CheckIn>>> GetAllCheckIns(CancellationToken cancellationToken)
        {
            var checkIns = await _checkInService.GetAllCheckInsAsync(cancellationToken);
            return Ok(checkIns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CheckIn>> GetCheckInById(Guid id, CancellationToken cancellationToken)
        {
            var checkIn = await _checkInService.GetCheckInByIdAsync(id, cancellationToken);
            if (checkIn == null)
            {
                return NotFound();
            }
            return Ok(checkIn);
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<List<CheckIn>>> GetCheckInsByEventId(Guid eventId, CancellationToken cancellationToken)
        {
            var checkIns = await _checkInService.GetCheckInsByEventIdAsync(eventId, cancellationToken);
            return Ok(checkIns);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<CheckIn>>> GetCheckInsByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var checkIns = await _checkInService.GetCheckInsByUserIdAsync(userId, cancellationToken);
            return Ok(checkIns);
        }

        [HttpPost]
        public async Task<ActionResult> AddCheckIn([FromBody] CheckIn checkInToAdd, CancellationToken cancellationToken)
        {
            var result = await _checkInService.AddCheckInAsync(checkInToAdd, cancellationToken);
            if (!result)
            {
                return BadRequest("Failed to add check-in.");
            }
            return CreatedAtAction(nameof(GetCheckInById), new { id = checkInToAdd.Id }, checkInToAdd);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCheckIn(Guid id, [FromBody] CheckIn checkInToUpdate, CancellationToken cancellationToken)
        {
            if (id != checkInToUpdate.Id)
            {
                return BadRequest("Check-In ID mismatch.");
            }

            var result = await _checkInService.UpdateCheckInAsync(checkInToUpdate, cancellationToken);
            if (!result)
            {
                return BadRequest("Failed to update check-in.");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCheckIn(Guid id, CancellationToken cancellationToken)
        {
            var result = await _checkInService.DeleteCheckInAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}