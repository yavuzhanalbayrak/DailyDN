namespace DailyDN.Application.Features.Auth.RefreshToken
{
    public record RefreshTokenCommandResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiration);
}