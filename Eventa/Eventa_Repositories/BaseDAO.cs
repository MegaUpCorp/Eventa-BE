using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories
{
    public class BaseDAO<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;
        public BaseDAO(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }
        public async Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity != null)
            {
                await _collection.InsertOneAsync(entity, null, cancellationToken);
                return true;
            }
            return false;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities.Any())
            {
                await _collection.InsertManyAsync(entities, null, cancellationToken);
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            return (int)await _collection.CountDocumentsAsync(filter ?? (x => true), null, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteManyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            var result = await _collection.DeleteManyAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            return await _collection.AsQueryable().Where(filter ?? (x => true)).ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _collection.AsQueryable().FirstOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<List<T>> GetWithPaginationAsync(int pageNum, int pageSize, Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            return await _collection.AsQueryable()
                .Where(filter ?? (x => true))
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions(), cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            bool isSuccess = true;
            foreach (var entity in entities)
            {
                var success = await UpdateAsync(entity, cancellationToken);
                if (!success)
                {
                    isSuccess = false;
                }
            }
            return isSuccess;
        }
    }
}
