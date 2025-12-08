using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : ICommand;
}