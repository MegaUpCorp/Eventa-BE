using Eventa_BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Interfaces
{
    public interface IOrganizerRepository : IRepository<Organizer>
    {
        Task<Organizer?> GetByAccountIdAsync(Guid accountId);
    }
}
