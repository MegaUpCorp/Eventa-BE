using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Guid id, string? includeProperties = null, CancellationToken cancellationToken = default);

        Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetWithPaginationAsync(int pageNum = 0, int pageSize = 0, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default);

        bool Update(T entity);

        bool UpdateRange(IEnumerable<T> entities);

        bool Delete(params T[] entities);
    }
}
