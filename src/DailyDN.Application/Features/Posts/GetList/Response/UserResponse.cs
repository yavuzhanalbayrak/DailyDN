namespace DailyDN.Application.Features.Posts.GetList.Response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? AvatarUrl { get; set; }
    }
}