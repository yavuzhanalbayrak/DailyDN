using DailyDN.Application.Features.Posts.GetList.Response;

namespace DailyDN.Application.Features.Posts.GetList
{
    public class GetListQueryResponse
    {
        public List<PostResponse> Posts { get; set; } = [];
    }
}