using DailyDN.Application.Common.Model;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Models;
using DailyDN.Infrastructure.UnitOfWork;
using DailyDN.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using DailyDN.Infrastructure.Helpers;
using DailyDN.Domain.Enums;

namespace DailyDN.Application.Services.Implementations
{
    public class AuthService(
        IUnitOfWork uow,
        IPasswordHasher<User> passwordHasher,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        IOtpService otpService,
        ISmsService smsService,
        IMailService mailService,
        IMailTemplateService mailTemplateService
    ) : IAuthService
    {
        public async Task<Result> LoginAsync(string email, string password)
        {
            var user = await uow.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
            {
                return Result.Failure(new Error("Unauthorized", "Email or password is incorrect."));
            }

            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return Result.Failure(new Error("Unauthorized", "Email or password is incorrect."));
            }

            var otpDto = otpService.CreateOtp();

            var otp = otpDto.Otp;
            var guid = otpDto.OtpGuid;

            await smsService.SendSmsAsync(user.PhoneNumber, otp.ToString());

            user.SetOtp(otp.ToString(), guid);

            await uow.Users.UpdateAsync(user);
            await uow.SaveChangesAsync();

            return Result.Success(new
            {
                Guid = guid.ToString(),
                Otp = otp // Fake sms provider olduğu için otp response içinde dönülüyor.
            });
        }

        public async Task<Result> RegisterAsync(
            string name,
            string surname,
            string email,
            string phoneNumber,
            string password,
            CancellationToken cancellationToken
        )
        {
            var isEmailExists = await uow.Users.GetAsync(u => u.Email == email);
            if (isEmailExists.Any())
            {
                return Result.Failure(new Error("Conflict", "This email is already registered."));
            }

            var isPhoneNumberExists = await uow.Users.GetAsync(u => u.PhoneNumber == phoneNumber);
            if (isPhoneNumberExists.Any())
            {
                return Result.Failure(new Error("Conflict", "This phone number is already registered."));
            }

            var hashedPassword = passwordHasher.HashPassword(null, password);

            var user = new User(
                name: name,
                surname: surname,
                email: email,
                phoneNumber: phoneNumber,
                passwordHash: hashedPassword,
                userRoles: [new(0, (int)RoleEnum.User)]
            );

            // var verifyLink = $"https://frontend-app/confirm-email?token={user.EmailConfirmationToken}";
            // var html = await mailTemplateService.GetTemplateAsync(
            //     "VerifyEmailTemplate.html",
            //     new Dictionary<string, string>
            //     {
            //         { "VERIFY_LINK", verifyLink }
            //     }
            // );

            // await mailService.SendEmailAsync(
            //     toList: [user.Email],
            //     subject: "Verify Your Email",
            //     body: html
            // );

            //TODO: email kontrolü için ayrı bir endpoint yazılacak.
            user.MarkEmailVerified();

            await uow.Users.AddAsync(user, cancellationToken);
            await uow.SaveChangesAsync();

            return Result.SuccessWithMessage("Registration completed successfully.");
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

        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            var requestRefreshTokenHash = HashHelper.HashSha256(refreshToken);

            var session = await uow.UserSessions.FirstOrDefaultAsync(us => us.RefreshTokenHash == requestRefreshTokenHash);
            if (session is null || !session.IsActive())
                return null;

            var newTokens = await tokenService.GenerateTokens(session.UserId, session.IpAddress, session.UserAgent);

            var newAccessToken = newTokens.AccessToken;
            var newRefreshTokenExpiry = newTokens.RefreshTokenExpiration;
            var newRefreshToken = newTokens.RefreshTokenHash;

            session.Revoke();
            await uow.UserSessions.UpdateAsync(session);
            await uow.SaveChangesAsync();

            return new TokenResponse(newAccessToken, newRefreshToken, newRefreshTokenExpiry, DateTime.Now);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await uow.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
                return;

            user.GeneratePasswordResetToken();
            await uow.Users.UpdateAsync(user);
            await uow.SaveChangesAsync();

            var resetLink = $"https://frontend-app/reset-password?token={user.ForgotPasswordToken}";
            var html = await mailTemplateService.GetTemplateAsync(
                "ResetPasswordTemplate.html",
                new Dictionary<string, string>
                {
                    { "RESET_LINK", resetLink }
                }
            );

            await mailService.SendEmailAsync(
                [user.Email],
                "Reset Your Password",
                html
            );

        }

        public async Task<Result> ResetPasswordAsync(Guid token, string newPassword)
        {
            var user = await uow.Users.FirstOrDefaultAsync(u => u.ForgotPasswordToken == token);
            if (user is null || !user.IsPasswordResetTokenValid(token, TimeSpan.FromMinutes(15)))
                return Result.Failure(new Error("Token.Invalid", "Token is invalid or has expired."));

            var hashedPassword = passwordHasher.HashPassword(null, newPassword);
            user.ResetPassword(hashedPassword);

            await uow.Users.UpdateAsync(user);
            await uow.SaveChangesAsync();

            return Result.SuccessWithMessage("Password reset successfully.");
        }
    }
}