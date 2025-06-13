namespace DailyDN.Infrastructure.Models
{
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}