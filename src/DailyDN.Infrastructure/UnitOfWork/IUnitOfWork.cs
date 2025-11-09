using DailyDN.Infrastructure.Repositories;

namespace DailyDN.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IUserSessionRepository UserSessions { get; }
        IPostRepository Posts { get; }
        Task<int> SaveChangesAsync();
    }
}