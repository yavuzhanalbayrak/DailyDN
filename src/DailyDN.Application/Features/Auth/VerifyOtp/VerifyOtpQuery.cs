using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.VerifyOtp
{
    public record VerifyOtpQuery(Guid Guid, string Otp) : IQuery<VerifyOtpQueryResponse>;
}