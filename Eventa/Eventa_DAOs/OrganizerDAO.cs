using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class OrganizerDAO : BaseDAO<Organizer>
    {
        public OrganizerDAO(IMongoDatabase database) : base(database, "Organizers") { }

        public async Task<Organizer?> GetOrganizerByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await GetAsync(id, cancellationToken);
        }

        public async Task<bool> AddOrganizerAsync(Organizer organizerToAdd, CancellationToken cancellationToken = default)
        {
            return await AddAsync(organizerToAdd, cancellationToken);
        }

        public async Task<bool> UpdateOrganizerAsync(Organizer organizerToUpdate, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(organizerToUpdate, cancellationToken);
        }

        public async Task<bool> DeleteOrganizerAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DeleteAsync(id, cancellationToken);
        }
    }
}
