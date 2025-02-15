using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Interfaces
{
    public interface ICheckInService
    {
        Task<List<CheckIn>> GetAllCheckInsAsync(CancellationToken cancellationToken = default);
        Task<CheckIn?> GetCheckInByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> AddCheckInAsync(CheckIn checkInToAdd, CancellationToken cancellationToken = default);
        Task<bool> UpdateCheckInAsync(CheckIn checkInToUpdate, CancellationToken cancellationToken = default);
        Task<bool> DeleteCheckInAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<CheckIn>> GetCheckInsByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task<List<CheckIn>> GetCheckInsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
