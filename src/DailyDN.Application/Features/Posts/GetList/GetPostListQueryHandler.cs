using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace DailyDN.Application.Features.Posts.GetList
{
    public class GetPostListQueryHandler(
        IPostsService postsService,
        IMapper mapper,
        ILogger<GetPostListQueryHandler> logger,
        IAuthenticatedUser authenticatedUser
    ) : IPaginatedQueryHandler<GetPostListQuery, GetPostListQueryResponse>
    {
        public async Task<PaginatedResult<GetPostListQueryResponse>> Handle(GetPostListQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting Posts by user: {@User}. Payload: {@Request}", authenticatedUser, request);

            var (posts, totalCount) = await postsService.GetListAsync(request.Page, request.PageSize);

            var postResponses = mapper.Map<GetPostListQueryResponse>(posts);

            return PaginatedResult<GetPostListQueryResponse>.Success(postResponses, totalCount, request.Page, request.PageSize);
        }
    }
}
