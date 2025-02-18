using Eventa_BusinessObject.DTOs.Organizer;
using Eventa_BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IOrganizerService
    {
        Task<ActionResult<Organizer?>> GetOrganizerByAccountId(Guid accountId);
        Task<ActionResult<List<Organizer>>> GetAllOrganizer();
        Task<ActionResult<Organizer?>> GetOrganizerById(Guid id);
        Task<string> AddOrganizer(Organizer organizer, HttpContext httpContext);
        Task<bool> UpdateOrganizerById(Guid id,UpdateOrganizerDTO updateOrganizerDTO);
        Task<bool> DeleteOrganizerById(Guid id);
    }
}
