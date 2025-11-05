using DailyDN.Domain.Entities;

namespace DailyDN.Application.Services.Interfaces;

public interface IPostsService
{
    public Task AddAsync(Post post, CancellationToken cancellationToken);
    public Task<(IEnumerable<Post> posts, int totalCount)> GetListAsync(int page, int pageSize);
}