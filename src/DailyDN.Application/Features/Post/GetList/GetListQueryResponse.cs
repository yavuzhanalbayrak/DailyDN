using DailyDN.Application.Features.Post.GetList.Response;

namespace DailyDN.Application.Features.Post.GetList
{
    public class GetListQueryResponse
    {
        public List<PostResponse> Posts { get; set; } = [];
    }
}