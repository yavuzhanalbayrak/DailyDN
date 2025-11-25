using DailyDN.Application.Common.Attributes;
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.ResetPassword
{
    public class ResetPasswordCommand(Guid token, string newPassword) : ICommand
    {
        [DoNotLog]
        public Guid Token { get; set; } = token;
        [DoNotLog]
        public string NewPassword { get; set; } = newPassword;
    }
}