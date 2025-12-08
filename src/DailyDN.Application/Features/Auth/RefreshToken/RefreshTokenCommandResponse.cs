namespace DailyDN.Application.Features.Auth.RefreshToken
{
    public record RefreshTokenCommandResponse(string AccessToken, string RefreshTokenHash, DateTime RefreshTokenExpiration);
}