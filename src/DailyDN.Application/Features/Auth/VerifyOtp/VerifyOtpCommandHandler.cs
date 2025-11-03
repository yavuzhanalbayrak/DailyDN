using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DailyDN.Application.Features.Auth.VerifyOtp
{
    public class VerifyOtpCommandHandler(
        IMapper mapper,
        ILogger<VerifyOtpCommandHandler> logger,
        IAuthService authService
    ) : ICommandHandler<VerifyOtpCommand>
    {
        public async Task<Result> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Otp service running.");

            var token = await authService.VerifyOtpAsync(request.Guid, request.Otp);

            if (token is null)
                return Result.Failure<VerifyOtpCommandResponse>(new Error("Otp.Invalid", "OTP is invalid or has expired."));

            var response = mapper.Map<VerifyOtpCommandResponse>(token);
            return Result.Success(response);
        }
    }
}