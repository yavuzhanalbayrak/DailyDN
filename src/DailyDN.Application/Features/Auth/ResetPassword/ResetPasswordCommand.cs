using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.ResetPassword
{
    public record ResetPasswordCommand(Guid Token, string NewPassword) : ICommand;
}