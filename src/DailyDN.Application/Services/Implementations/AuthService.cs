using DailyDN.Application.Common.Model;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Models;
using DailyDN.Infrastructure.UnitOfWork;
using DailyDN.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using DailyDN.Infrastructure.Helpers;
using DailyDN.Domain.ValueObjects;

namespace DailyDN.Application.Services.Implementations
{
    public class AuthService(
        IUnitOfWork uow,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        ISmsService smsService,
        IMailService mailService,
        IMailTemplateService mailTemplateService,
        IOtpService otpService
    ) : IAuthService
    {
        public async Task<Result> LoginAsync(string email, string password)
        {
            var user = await uow.Users.FirstOrDefaultAsync(u => u.Email.Value == email);
            if (user is null)
            {
                return Result.Failure(new Error("Unauthorized", "Email or password is incorrect."));
            }

            if (!user.IsEmailVerified)
            {
                return Result.Failure(new Error("EmailNotVerified", "Your email address is not verified. Please verify your email before logging in."));
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
            var isEmailExists = await uow.Users.GetAsync(u => u.Email.Value == email);
            if (isEmailExists.Any())
            {
                return Result.Failure(new Error("Conflict", "This email is already registered."));
            }

            var isPhoneNumberExists = await uow.Users.GetAsync(u => u.PhoneNumber.Value == phoneNumber);
            if (isPhoneNumberExists.Any())
            {
                return Result.Failure(new Error("Conflict", "This phone number is already registered."));
            }

            var user = new User(
                fullName: new FullName(name, surname),
                email: new Email(email),
                phoneNumber: new PhoneNumber(phoneNumber),
                passwordHash: new PasswordHash()
            );

            user.AddRole(Domain.Enums.Role.User);

            var hashedPassword = passwordHasher.HashPassword(user, password);
            user.SetPassword(hashedPassword);

            user.GenerateEmailVerificationToken();

            var verifyLink = $"https://frontend-app/confirm-email?token={user.EmailVerificationToken}";
            var html = await mailTemplateService.GetTemplateAsync(
                "VerifyEmailTemplate.html",
                new Dictionary<string, string>
                {
                    { "VERIFY_LINK", verifyLink }
                }
            );

            await mailService.SendEmailAsync(
                toList: [user.Email.Value],
                subject: "Verify Your Email",
                body: html
            );

            await uow.Users.AddAsync(user, cancellationToken);
            await uow.SaveChangesAsync();

            return Result.SuccessWithMessage("Registration completed successfully.");
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
            var user = await uow.Users.FirstOrDefaultAsync(u => u.Email.Value == email);
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

            var hashedPassword = passwordHasher.HashPassword(user, newPassword);
            user.ResetPassword(hashedPassword);

            await uow.Users.UpdateAsync(user);
            await uow.SaveChangesAsync();

            return Result.SuccessWithMessage("Password reset successfully.");
        }

        public async Task<Result> VerifyEmailAsync(Guid guid)
        {
            var user = await uow.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == guid);

            if (user == null)
                return Result.Failure(new Error("NotFound", "Invalid verification token."));

            try
            {
                user.VerifyEmailToken(guid, TimeSpan.FromHours(24));
                await uow.Users.UpdateAsync(user);
                await uow.SaveChangesAsync();
                return Result.SuccessWithMessage("Email verified successfully.");
            }
            catch (Exception)
            {
                return Result.Failure(new Error("Conflict", "Invalid verification token."));
            }
        }
    }
}