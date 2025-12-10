using DailyDN.Domain.ValueObjects;

namespace DailyDN.Application.Features.Posts.GetList.Response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public FullName FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
    }
}