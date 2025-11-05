using DailyDN.Application.Features.Posts.GetList.Response;

namespace DailyDN.Application.Features.Posts.GetList
{
    public class GetPostListQueryResponse
    {
        public List<PostResponse> Posts { get; set; } = [];
    }
}