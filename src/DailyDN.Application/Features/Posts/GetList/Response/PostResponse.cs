using DailyDN.Domain.Entities;

namespace DailyDN.Application.Features.Posts.GetList.Response
{
    public class PostResponse
    {
        public int Id { get; set; }
        public string Caption { get; set; } = null!;
        public string MediaUrl { get; set; } = null!;
        public string MediaType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public UserResponse User { get; set; } = null!;
    }
}