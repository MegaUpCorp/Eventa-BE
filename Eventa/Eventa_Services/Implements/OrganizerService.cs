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
    public class OrganizerService : IOrganizerService
    {
        private readonly OrganizerDAO _organizerDAO;

        public OrganizerService(IMongoDatabase database)
        {
            _organizerDAO = new OrganizerDAO(database);
        }

        public async Task<Organizer?> GetOrganizerByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _organizerDAO.GetOrganizerByIdAsync(id, cancellationToken);
        }

        public async Task<bool> AddOrganizerAsync(Organizer organizerToAdd, CancellationToken cancellationToken = default)
        {
            return await _organizerDAO.AddOrganizerAsync(organizerToAdd, cancellationToken);
        }

        public async Task<bool> UpdateOrganizerAsync(Organizer organizerToUpdate, CancellationToken cancellationToken = default)
        {
            return await _organizerDAO.UpdateOrganizerAsync(organizerToUpdate, cancellationToken);
        }

        public async Task<bool> DeleteOrganizerAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _organizerDAO.DeleteOrganizerAsync(id, cancellationToken);
        }
    }
}
