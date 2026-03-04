using System.Linq.Expressions;

namespace Clikh.UnitOfWork
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();
        Task<T?> FindAsync(params object?[]? keyValues);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities, int batchSize = 2000);
        void ClearTracker();
        void Update(T entity);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
    }
}
