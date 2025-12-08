using DailyDN.Domain.Entities;

namespace DailyDN.Infrastructure.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserWithRolesAsync(int id);
        Task<User?> GetUserWithRolesAndClaimsAsync(int id);
    }
}