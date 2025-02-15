using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface IOrganizerService
    {
        Task<Organizer?> GetOrganizerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> AddOrganizerAsync(Organizer organizerToAdd, CancellationToken cancellationToken = default);
        Task<bool> UpdateOrganizerAsync(Organizer organizerToUpdate, CancellationToken cancellationToken = default);
        Task<bool> DeleteOrganizerAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
