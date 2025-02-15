using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Services.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class CheckInService : ICheckInService
    {
        private readonly CheckInDAO _checkInDAO;

        public CheckInService(IMongoDatabase database)
        {
            _checkInDAO = new CheckInDAO(database);
        }

        public async Task<List<CheckIn>> GetAllCheckInsAsync(CancellationToken cancellationToken = default)
        {
            return await _checkInDAO.GetAllAsync(null, cancellationToken);
        }

        public async Task<CheckIn?> GetCheckInByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _checkInDAO.GetCheckInByIdAsync(id, cancellationToken);
        }

        public async Task<bool> AddCheckInAsync(CheckIn checkInToAdd, CancellationToken cancellationToken = default)
        {
            return await _checkInDAO.AddCheckInAsync(checkInToAdd, cancellationToken);
        }

        public async Task<bool> UpdateCheckInAsync(CheckIn checkInToUpdate, CancellationToken cancellationToken = default)
        {
            return await _checkInDAO.UpdateCheckInAsync(checkInToUpdate, cancellationToken);
        }

        public async Task<bool> DeleteCheckInAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _checkInDAO.DeleteCheckInAsync(id, cancellationToken);
        }

        public async Task<List<CheckIn>> GetCheckInsByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await _checkInDAO.GetCheckInsByEventIdAsync(eventId, cancellationToken);
        }

        public async Task<List<CheckIn>> GetCheckInsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _checkInDAO.GetCheckInsByUserIdAsync(userId, cancellationToken);
        }
    }
}
