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
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Event>>> GetAllEvents(CancellationToken cancellationToken)
        {
            var events = await _eventService.GetAllEventsAsync(cancellationToken);
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEventById(Guid id, CancellationToken cancellationToken)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id, cancellationToken);
            if (eventItem == null)
            {
                return NotFound();
            }
            return Ok(eventItem);
        }

        [HttpPost]
        public async Task<ActionResult> AddEvent([FromBody] Event eventToAdd, CancellationToken cancellationToken)
        {
            var result = await _eventService.AddEventAsync(eventToAdd, cancellationToken);
            if (!result)
            {
                return BadRequest("Invalid OrganizerId or OwnerId.");
            }
            return CreatedAtAction(nameof(GetEventById), new { id = eventToAdd.Id }, eventToAdd);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] Event eventToUpdate, CancellationToken cancellationToken)
        {
            if (id != eventToUpdate.Id)
            {
                return BadRequest("Event ID mismatch.");
            }

            var result = await _eventService.UpdateEventAsync(eventToUpdate, cancellationToken);
            if (!result)
            {
                return BadRequest("Invalid OrganizerId or OwnerId.");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id, CancellationToken cancellationToken)
        {
            var result = await _eventService.DeleteEventAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}