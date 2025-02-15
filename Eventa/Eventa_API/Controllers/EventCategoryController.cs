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
    public class EventCategoryController : ControllerBase
    {
        private readonly IEventCategoryService _eventCategoryService;

        public EventCategoryController(IEventCategoryService eventCategoryService)
        {
            _eventCategoryService = eventCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventCategory>>> GetAllEventCategories(CancellationToken cancellationToken)
        {
            var eventCategories = await _eventCategoryService.GetAllEventCategoriesAsync(cancellationToken);
            return Ok(eventCategories);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<EventCategory>> GetEventCategoryById(Guid id, CancellationToken cancellationToken)
        {
            var eventCategory = await _eventCategoryService.GetEventCategoryByIdAsync(id, cancellationToken);
            if (eventCategory == null)
            {
                return NotFound();
            }
            return Ok(eventCategory);
        }

        [HttpPost]
        public async Task<ActionResult> AddEventCategory([FromBody] EventCategory eventCategoryToAdd, CancellationToken cancellationToken)
        {
            var result = await _eventCategoryService.AddEventCategoryAsync(eventCategoryToAdd, cancellationToken);
            if (!result)
            {
                return BadRequest("Failed to add event category.");
            }
            return CreatedAtAction(nameof(GetEventCategoryById), new { id = eventCategoryToAdd.Id }, eventCategoryToAdd);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEventCategory(Guid id, [FromBody] EventCategory eventCategoryToUpdate, CancellationToken cancellationToken)
        {
            if (id != eventCategoryToUpdate.Id)
            {
                return BadRequest("Event Category ID mismatch.");
            }

            var result = await _eventCategoryService.UpdateEventCategoryAsync(eventCategoryToUpdate, cancellationToken);
            if (!result)
            {
                return BadRequest("Failed to update event category.");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEventCategory(Guid id, CancellationToken cancellationToken)
        {
            var result = await _eventCategoryService.DeleteEventCategoryAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}