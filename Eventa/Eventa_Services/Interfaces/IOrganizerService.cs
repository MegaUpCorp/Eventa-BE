using Eventa_BusinessObject.Entities;
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
        Task<bool> AddOrganizer(Organizer organizer);
        Task<bool> UpdateOrganizerById(Organizer organizer);
        Task<bool> DeleteOrganizerById(Guid id);
    }
}
