using DailyDN.Domain.Entities;

namespace DailyDN.Application.Services.Impl
{
    public class AuthenticatedUser : IAuthenticatedUser
    {
        public int UserId { get; set; }
        public bool IsAuthenticated { get; set; }
        public Role Role { get; set; } = null!;
        public List<Claim> Claims { get; set; } = [];
    }
}