using Eventa_BusinessObject.Entities;
using Eventa_DAOs;
using Eventa_Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Implements
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDAO _notificationDAO;

        public NotificationRepository(NotificationDAO notificationDAO)
        {
            _notificationDAO = notificationDAO;
        }

        public Task<bool> AddAsync(Notification entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<Notification> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<Notification, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(params Notification[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<List<Notification>> GetAllAsync(Expression<Func<Notification, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Notification?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Notification?> GetAsync(Expression<Func<Notification, bool>> filter, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<Notification>> GetByAccountIdAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Notification entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRange(IEnumerable<Notification> entities)
        {
            throw new NotImplementedException();
        }
    }
}
