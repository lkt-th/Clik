using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;

namespace Clik.UnitOfWork
{
    public class UnitOfWork(DbContext db) : IUnitOfWork, IAsyncDisposable, IDisposable
    {
        private readonly ConcurrentDictionary<Type, object> _repositories = new();
        private IDbContextTransaction? _currentTransaction;
        private bool _disposed = false;

        public IRepository<T> Repository<T>() where T : class
        {
            return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ =>
            {
                var repositoryType = typeof(Repository<>).MakeGenericType(typeof(T));
                return Activator.CreateInstance(repositoryType, db)
                    ?? throw new InvalidOperationException($"Could not create repository instance for {typeof(T).Name}");
            });
        }

        public async Task<int> SaveAsync(CancellationToken ct = default)
            => await db.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await db.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            try
            {
                await db.SaveChangesAsync(ct);
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(ct);
                }
            }
            catch
            {
                await RollbackTransactionAsync(ct);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(ct);
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        // เพิ่ม IAsyncDisposable สำหรับ DB Context รุ่นใหม่ๆ
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_currentTransaction != null) await _currentTransaction.DisposeAsync();
                    // db.Dispose() ปกติ DI จัดการให้ แต่ถ้าเขียนเผื่อ Manual Instantiate ก็ใส่ไว้ได้
                    await db.DisposeAsync();
                    _repositories.Clear();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            db.Dispose();
            _repositories.Clear();
            GC.SuppressFinalize(this);
        }
    }
}