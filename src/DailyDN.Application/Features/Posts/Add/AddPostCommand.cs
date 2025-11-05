using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Posts.Add
{
    public class AddPostCommand : ICommand
    {
        public string Caption { get; set; } = null!;
        public string MediaUrl { get; set; } = null!;
        public string MediaType { get; set; } = null!;
    }
}