namespace DailyDN.Infrastructure.Services
{
    public interface IAuthenticatedUser
    {
        public int UserId { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Role { get; set; }
        public List<string> Claims { get; set; }
    }
}