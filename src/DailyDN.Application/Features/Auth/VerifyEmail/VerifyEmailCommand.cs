using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.VerifyEmail
{
    public record VerifyEmailCommand(Guid Token) : ICommand;
}