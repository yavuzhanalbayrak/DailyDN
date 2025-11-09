using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.Application.Services.Implementations;

public class PostService(
    IUnitOfWork uow
) : IPostsService
{
    public async Task AddAsync(Post post, CancellationToken cancellationToken)
    {
        await uow.Posts.AddAsync(post, cancellationToken);
        await uow.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Post> posts, int totalCount)> GetListAsync(int page, int pageSize)
    {
        var includes = new Func<IQueryable<Post>, IQueryable<Post>>(query =>
            query.Include(p => p.User)
        );

        return await uow.Posts.GetPaginatedAsync(
            page,
            pageSize,
            includes,
            orderBy: p => p.CreatedAt,
            orderDescending: true
        );
    }
}