using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Contexts;
using DailyDN.Infrastructure.Repositories;
using DailyDN.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace DailyDN.Application.Features.Auth.VerifyOtp
{
    public class VerifyOtpCommandHandler(
        IGenericRepository<User> userRepository,
        ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper
    ) : ICommandHandler<VerifyOtpCommand>
    {
        public async Task<Result> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";
            var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? "";

            var userList = await userRepository.GetAsync(u => u.Guid == request.Guid);
            var user = userList[0];

            if (user.IsOtpValid(user.OtpCode, TimeSpan.FromMinutes(1)))
            {
                var tokenResponse = await tokenService.GenerateTokens(user.Id, ipAddress, userAgent);
                var response = mapper.Map<VerifyOtpCommandResponse>(tokenResponse);

                user.IsGuidUsed = true;
                await userRepository.UpdateAsync(user);

                return Result.Success(response);
            }
            else
                return Result.Failure<VerifyOtpCommandResponse>(new Error("Otp.Invalid", "OTP is invalid or has expired."));
        }
    }
}