using DailyDN.Application.Common.Attributes;
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.VerifyOtp
{
    public record VerifyOtpCommand(Guid Guid, string Otp) : ICommand<VerifyOtpCommandResponse>
    {
        [DoNotLog]
        public Guid Guid { get; set; } = Guid;
        [DoNotLog]
        public string Otp { get; set; } = Otp;
    }
}