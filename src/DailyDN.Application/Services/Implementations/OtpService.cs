using DailyDN.Application.Dtos.Otp;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Infrastructure.Models;
using DailyDN.Infrastructure.Services;
using DailyDN.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace DailyDN.Application.Services.Implementations
{
    public class OtpService(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork uow,
        ITokenService tokenService
    ) : IOtpService
    {
        public OtpDto CreateOtp()
        {
            return new OtpDto();
        }

        public async Task<TokenResponse?> VerifyOtpAsync(Guid guid, string otp)
        {
            var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";
            var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? "";

            var userList = await uow.Users.GetAsync(u => u.Guid == guid);
            var user = userList[0];

            if (user.IsOtpValid(otp, TimeSpan.FromMinutes(1)))
            {
                var tokenResponse = await tokenService.GenerateTokens(user.Id, ipAddress, userAgent);

                user.Login();
                await uow.Users.UpdateAsync(user);
                await uow.SaveChangesAsync();

                return tokenResponse;
            }
            else
                return null;

        }
    }
}