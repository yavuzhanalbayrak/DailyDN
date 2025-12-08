namespace DailyDN.Application.Features.Auth.VerifyOtp
{
    public class VerifyOtpCommandResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; } 
        public string RefreshTokenHash { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; }
    }
}