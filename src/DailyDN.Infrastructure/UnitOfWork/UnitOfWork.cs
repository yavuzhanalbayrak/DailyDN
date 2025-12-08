using DailyDN.Infrastructure.Contexts;
using DailyDN.Infrastructure.Repositories;

namespace DailyDN.Infrastructure.UnitOfWork
{
    public class UnitOfWork(
        IApplicationContext context,
        IUserRepository users,
        IUserSessionRepository userSessions,
        IPostRepository posts
        ) : IUnitOfWork
    {
        private readonly IApplicationContext _context = context;
        private bool _disposed;
        public IUserRepository Users { get; } = users;

        public IUserSessionRepository UserSessions { get; } = userSessions;

        public IPostRepository Posts { get; } = posts;

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }
    }
}