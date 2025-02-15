using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
        public class EventDAO : BaseDAO<Event>
        {
            public EventDAO(IMongoDatabase database) : base(database, "Events") { }

            // Lấy tất cả sự kiện với bộ lọc tùy chọn
            public async Task<List<Event>> GetAllEventsAsync(Expression<Func<Event, bool>>? filter = null, CancellationToken cancellationToken = default)
            {
                return await GetAllAsync(filter, cancellationToken);
            }

            // Lấy sự kiện theo ID
            public async Task<Event?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default)
            {
                return await GetAsync(id, cancellationToken);
            }

            // Lấy sự kiện theo một điều kiện
            public async Task<Event?> GetEventByConditionAsync(Expression<Func<Event, bool>> filter, CancellationToken cancellationToken = default)
            {
                return await GetAsync(filter, cancellationToken);
            }

            // Cập nhật sự kiện
            public async Task<bool> UpdateEventAsync(Event eventToUpdate, CancellationToken cancellationToken = default)
            {
                return await UpdateAsync(eventToUpdate, cancellationToken);
            }

            // Xóa sự kiện theo ID
            public async Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken = default)
            {
                return await DeleteAsync(id, cancellationToken);
            }

            // Xóa nhiều sự kiện theo bộ lọc
            public async Task<bool> DeleteEventsAsync(Expression<Func<Event, bool>> filter, CancellationToken cancellationToken = default)
            {
                return await DeleteManyAsync(filter, cancellationToken);
            }

            // Thêm sự kiện mới
            public async Task<bool> AddEventAsync(Event eventToAdd, CancellationToken cancellationToken = default)
            {
                return await AddAsync(eventToAdd, cancellationToken);
            }

            // Lấy sự kiện với phân trang
            public async Task<List<Event>> GetEventsWithPaginationAsync(int pageNum, int pageSize, Expression<Func<Event, bool>>? filter = null, CancellationToken cancellationToken = default)
            {
                return await GetWithPaginationAsync(pageNum, pageSize, filter, cancellationToken);
            }
        }
}
