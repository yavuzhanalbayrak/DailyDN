namespace DailyDN.Application.Services.Impl
{
    public class AuthenticatedUser : IAuthenticatedUser
    {
        public int UserId { get; set; }
        public bool IsAuthenticated { get; set; }
        // public List<Claim> Claims { get; set; }
    }
}