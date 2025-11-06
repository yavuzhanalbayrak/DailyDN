namespace DailyDN.Infrastructure.Models
{
    public class TokenResponse(string accessToken, string refreshToken, DateTime accessTokenExpiration, DateTime refreshTokenExpiration)
    {
        public string AccessToken { get; set; } = accessToken;
        public DateTime AccessTokenExpiration { get; set; } = accessTokenExpiration;
        public DateTime RefreshTokenExpiration { get; set; } = refreshTokenExpiration;
        public string RefreshToken { get; set; } = refreshToken;
    }
}