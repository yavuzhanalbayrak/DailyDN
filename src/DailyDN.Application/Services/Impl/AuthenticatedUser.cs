using DailyDN.Domain.Entities;

namespace DailyDN.Application.Services.Impl
{
    public class AuthenticatedUser : IAuthenticatedUser
    {
        public int UserId { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Role { get; set; } = null!;
        public List<string> Claims { get; set; } = [];
    }
}