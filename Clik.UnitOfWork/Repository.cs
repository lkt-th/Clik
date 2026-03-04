using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Clikh.UnitOfWork
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _db;
        private readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _db = context;
            _dbSet = _db.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, int batchSize = 2000)
        {
            _db.ChangeTracker.AutoDetectChangesEnabled = false;
            var chunks = entities.Chunk(batchSize);
            foreach (var chunk in chunks)
            {
                await _dbSet.AddRangeAsync(chunk);
                await _db.SaveChangesAsync();
                ClearTracker();
            }
            _db.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        public IQueryable<T> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public void ClearTracker() => _db.ChangeTracker.Clear();

        public Task<T?> FindAsync(params object?[]? keyValues)
        {
            if (keyValues == null)
                return Task.FromResult<T?>(null);
            return _dbSet.FindAsync(keyValues).AsTask();
        }

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefaultAsync(predicate);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task UpdateBulkAsync(Expression<Func<T, bool>> predicate, Action<UpdateSettersBuilder<T>> setter)
        {
            await _dbSet.Where(predicate).ExecuteUpdateAsync(setter);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task DeleteBulkAsync(Expression<Func<T, bool>> predicate)
        {
            await _dbSet.Where(predicate).ExecuteDeleteAsync();
        }
    }
}
