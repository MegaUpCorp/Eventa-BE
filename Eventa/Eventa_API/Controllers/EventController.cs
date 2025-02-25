using Eventa_BusinessObject.DTOs.Event;
using Eventa_BusinessObject.Entities;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventa_API.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        [HttpGet("get-all")]
        public async Task<ActionResult<List<Event>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEvents();
            return Ok(events);
        }
        [HttpGet("get-all-user")]
        public async Task<ActionResult<List<Event>>> GetAllEventOfUser()
        {
            var events = await _eventService.GetEventsByAccountId(HttpContext);
            return  Ok(events);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Event>> GetEventById(Guid id)
        {
            var eventItem = await _eventService.GetEventById(id);
            if (eventItem == null)
            {
                return NotFound("Event not found");
            }
            return Ok(eventItem);
        }
        [HttpPost("create")]
        public async Task<ActionResult<string>> CreateEvent([FromBody] CreateEventDTO eventDto)
        {
            var result = await _eventService.AddEvent(eventDto, HttpContext);
            if (result == "Account not found" || result == "Failed to add organizer")
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventDTO eventDto)
        {
            var isUpdated = await _eventService.UpdateEvent(id, eventDto);
            if (!isUpdated)
            {
                return NotFound("Event not found or update failed");
            }
            return Ok("Event updated successfully");
        }
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id)
        {
            var isRemoved = await _eventService.RemoveEvent(id);
            if (!isRemoved)
            {
                return NotFound("Event not found or could not be deleted");
            }
            return Ok("Event deleted successfully");
        }
    }
}
