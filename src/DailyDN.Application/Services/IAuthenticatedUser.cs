namespace DailyDN.Application.Services
{
    public interface IAuthenticatedUser
    {
        public int UserId { get; set; }
        public bool IsAuthenticated { get; set; }
        // public List<Claim> Claims { get; set; }
    }
}