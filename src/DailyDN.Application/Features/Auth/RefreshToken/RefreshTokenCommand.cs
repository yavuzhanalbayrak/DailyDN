using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenCommandResponse>;
}