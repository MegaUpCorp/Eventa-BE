using Eventa_BusinessObject.DTOs.Organizer;
using Eventa_BusinessObject.Entities;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizerController : ControllerBase
    {
        private readonly IOrganizerService organizerService;
        public OrganizerController(IOrganizerService organizerService)
        {
            this.organizerService = organizerService;
        }
        [HttpGet]
        public async Task<ActionResult<List<Organizer>>> GetAllOrganizer()
        {
            return await organizerService.GetAllOrganizer();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Organizer>> GetOrganizerById([FromRoute] Guid id)
        {
            var organizer = await organizerService.GetOrganizerById(id);
            if (organizer == null)
            {
                return NotFound("Organizer not found");
            }
            return Ok(organizer);
        }
        [HttpGet("accountId")]
        public async Task<ActionResult<Organizer>> GetOrganizerByAccountId([FromQuery] Guid accountId)
        {
            var organizer = await organizerService.GetOrganizerByAccountId(accountId);
            if (organizer == null)
            {
                return NotFound("Organizer not found");
            }
            return Ok(organizer);
        }
        [HttpPost]
        public async Task<ActionResult<string>> AddOrganizer([FromBody] Organizer organizer)
        {
            var result = await organizerService.AddOrganizer(organizer, HttpContext);
            if (result == "Account not found" || result == "Failed to add organizer")
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult<bool>> UpdateOrganizerById([FromRoute] Guid id, [FromBody] UpdateOrganizerDTO updateOrganizerDTO)
        {
            var isUpdated = await organizerService.UpdateOrganizerById(id, updateOrganizerDTO);
            if (!isUpdated)
            {
                return NotFound("Organizer not found or update failed");
            }
            return Ok(isUpdated);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteOrganizerById([FromRoute] Guid id)
        {
            var isRemoved = await organizerService.DeleteOrganizerById(id);
            if (!isRemoved)
            {
                return NotFound("Organizer not found or delete failed");
            }
            return Ok(isRemoved);
        }
    }
}
