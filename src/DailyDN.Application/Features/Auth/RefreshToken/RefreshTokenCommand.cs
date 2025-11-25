using DailyDN.Application.Common.Attributes;
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.RefreshToken
{
    public class RefreshTokenCommand(string refreshToken) : ICommand<RefreshTokenCommandResponse>
    {
        [DoNotLog]
        public string RefreshToken { get; set; } = refreshToken;
    }
}