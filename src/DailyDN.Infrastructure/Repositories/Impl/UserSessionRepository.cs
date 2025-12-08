using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Contexts;
using Microsoft.Extensions.Logging;

namespace DailyDN.Infrastructure.Repositories.Impl
{
    public class UserSessionRepository(IApplicationContext context, ILogger<UserSessionRepository> logger) : GenericRepository<UserSession>(context, logger), IUserSessionRepository
    {
        
    }
}