using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;

namespace DailyDN.Application.Features.Posts.GetList
{
    public class GetPostListQueryHandler(
        IPostsService postsService,
        IMapper mapper
    ) : IPaginatedQueryHandler<GetPostListQuery, GetPostListQueryResponse>
    {
        public async Task<PaginatedResult<GetPostListQueryResponse>> Handle(GetPostListQuery request, CancellationToken cancellationToken)
        {
            var (posts, totalCount) = await postsService.GetListAsync(request.Page, request.PageSize);

            var postResponses = mapper.Map<GetPostListQueryResponse>(posts);

            return PaginatedResult<GetPostListQueryResponse>.Success(postResponses, totalCount, request.Page, request.PageSize);
        }
    }
}
