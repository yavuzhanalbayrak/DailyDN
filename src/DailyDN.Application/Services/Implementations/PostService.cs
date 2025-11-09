using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.Application.Services.Implementations;

public class PostService(
    IPostRepository postRepository
) : IPostsService
{
    public async Task AddAsync(Post post, CancellationToken cancellationToken)
    {
        await postRepository.AddAsync(post, cancellationToken);
    }

    public async Task<(IEnumerable<Post> posts, int totalCount)> GetListAsync(int page, int pageSize)
    {
        var includes = new Func<IQueryable<Post>, IQueryable<Post>>(query =>
            query.Include(p => p.User)
        );

        return await postRepository.GetPaginatedAsync(
            page,
            pageSize,
            includes,
            orderBy: p => p.CreatedAt,
            orderDescending: true
        );
    }
}