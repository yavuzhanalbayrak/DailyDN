using DailyDN.Application.Common.Attributes;
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.VerifyEmail
{
    public class VerifyEmailCommand(Guid token) : ICommand
    {
        [DoNotLog]
        public Guid Token { get; set; } = token;
    }
}