using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DailyDN.Infrastructure.Repositories.Impl
{
    public class UserRepository(IApplicationContext context, ILogger<UserRepository> logger) : GenericRepository<User>(context, logger), IUserRepository
    {
        public async Task<User?> GetUserWithRolesAsync(int id)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserWithRolesAndClaimsAsync(int id)
        {
            return await context.Set<User>()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RoleClaims)
                            .ThenInclude(rc => rc.Claim)
                .FirstAsync(u => u.Id == id);
        }
    }
}