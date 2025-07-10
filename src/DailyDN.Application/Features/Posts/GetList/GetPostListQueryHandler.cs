using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Features.Posts.GetList.Response;
using DailyDN.Application.Messaging;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.Application.Features.Posts.GetList
{
    public class GetPostListQueryHandler(IGenericRepository<Post> postRepository, IMapper mapper) : IPaginatedQueryHandler<GetPostListQuery, GetPostListQueryResponse>
    {
        public async Task<PaginatedResult<GetPostListQueryResponse>> Handle(GetPostListQuery request, CancellationToken cancellationToken)
        {
            var includes = new Func<IQueryable<Post>, IQueryable<Post>>(query =>
                query.Include(p => p.User)
            );
            
            var (posts, totalCount) = await postRepository.GetPaginatedAsync(
                request.Page,
                request.PageSize,
                includes,
                orderBy: p => p.CreatedAt,
                orderDescending: true
            );

            var postResponses = mapper.Map<GetPostListQueryResponse>(posts);

            return PaginatedResult<GetPostListQueryResponse>.Success(postResponses, totalCount, request.Page, request.PageSize);
        }
    }
}
