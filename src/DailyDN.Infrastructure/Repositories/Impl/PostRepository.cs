using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Contexts;
using Microsoft.Extensions.Logging;

namespace DailyDN.Infrastructure.Repositories.Impl
{
    public class PostRepository(IApplicationContext context, ILogger<PostRepository> logger) : GenericRepository<Post>(context, logger), IPostRepository
    {
        
    }
}