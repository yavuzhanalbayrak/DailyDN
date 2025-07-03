namespace DailyDN.Application.Features.Auth.VerifyOtp
{
    public class VerifyOtpQueryResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}