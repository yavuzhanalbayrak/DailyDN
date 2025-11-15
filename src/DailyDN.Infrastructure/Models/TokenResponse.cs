namespace DailyDN.Infrastructure.Models
{
    public class TokenResponse(string accessToken, string refreshTokenHash, DateTime accessTokenExpiration, DateTime refreshTokenExpiration)
    {
        public string AccessToken { get; set; } = accessToken;
        public DateTime AccessTokenExpiration { get; set; } = accessTokenExpiration;
        public DateTime RefreshTokenExpiration { get; set; } = refreshTokenExpiration;
        public string RefreshTokenHash { get; set; } = refreshTokenHash;
    }
}