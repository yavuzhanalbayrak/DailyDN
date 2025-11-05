using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Posts.GetList
{
    public class GetPostListQuery : IPaginatedQuery<GetPostListQueryResponse>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}