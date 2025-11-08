using System.Security.Cryptography;
using System.Text;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Models;
using DailyDN.Infrastructure.Repositories;
using DailyDN.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DailyDN.Application.Services.Implementations
{
    public class AuthService(
        IGenericRepository<User> userRepository,
        IGenericRepository<UserSession> sessionRepository,
        IPasswordHasher<User> passwordHasher,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService
    ) : IAuthService
    {
        public async Task<Result> LoginAsync(string email, string password)
        {
            var user = await userRepository.GetAsync(u => u.Email == email);
            if (!user.Any())
            {
                return Result.Failure(new Error("Unauthorized", "Email or password is incorrect."));
            }

            var userEntity = user[0];
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(userEntity, userEntity.PasswordHash, password);

            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return Result.Failure(new Error("Unauthorized", "Email or password is incorrect."));
            }

            var otp = Random.Shared.Next(100000, 999999);
            var guid = Guid.NewGuid();

            userEntity.SetOtp(otp.ToString(), guid);

            await userRepository.UpdateAsync(userEntity);

            return Result.Success(new
            {
                Guid = guid.ToString(),
                Otp = otp // Sms servis yok, response içinde dönülüyor
            });
        }

        public async Task<Result> RegisterAsync(
            string name,
            string surname,
            string email,
            string password,
            CancellationToken cancellationToken
        )
        {
            var exists = await userRepository.GetAsync(u => u.Email == email);
            if (exists.Any())
            {
                return Result.Failure(new Error("Conflict", "This email is already registered."));
            }

            var hashedPassword = passwordHasher.HashPassword(null, password);

            var user = new User(
                name: name,
                surname: surname,
                email: email,
                passwordHash: hashedPassword
            );

            //TODO: email kontrolü için ayrı bir endpoint yazılacak.
            user.MarkEmailVerified();

            await userRepository.AddAsync(user, cancellationToken);

            return Result.SuccessWithMessage("Registration completed successfully.");
        }

        public async Task<TokenResponse?> VerifyOtpAsync(Guid guid, string otp)
        {
            var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";
            var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? "";

            var userList = await userRepository.GetAsync(u => u.Guid == guid);
            var user = userList[0];

            if (user.IsOtpValid(otp, TimeSpan.FromMinutes(1)))
            {
                var tokenResponse = await tokenService.GenerateTokens(user.Id, ipAddress, userAgent);

                user.Login();
                await userRepository.UpdateAsync(user);

                return tokenResponse;
            }
            else
                return null;

        }

        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            var requestRefreshTokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));

            var session = (await sessionRepository.GetAsync(us => us.RefreshToken == requestRefreshTokenHash))[0];
            if (session is null || !session.IsActive())
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var newTokens = await tokenService.GenerateTokens(session.UserId, session.IpAddress, session.UserAgent);

            var newAccessToken = newTokens.AccessToken;
            var newRefreshTokenExpiry = newTokens.RefreshTokenExpiration;
            var newRefreshToken = newTokens.RefreshToken;

            session.Revoke();
            await sessionRepository.UpdateAsync(session);

            return new TokenResponse(newAccessToken, newRefreshToken, newRefreshTokenExpiry, DateTime.Now);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = (await userRepository.GetAsync(u => u.Email == email)).FirstOrDefault();
            if (user is null)
                return;

            user.GeneratePasswordResetToken();
            await userRepository.UpdateAsync(user);

            // var resetLink = $"https://frontend-app/reset-password?token={user.Guid}";
            // await emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
        }

        public async Task<Result> ResetPasswordAsync(Guid token, string newPassword)
        {
            var user = (await userRepository.GetAsync(u => u.ForgotPasswordToken == token)).FirstOrDefault();
            if (user is null || !user.IsPasswordResetTokenValid(token, TimeSpan.FromMinutes(15)))
                return Result.Failure(new Error("Token.Invalid", "Token is invalid or has expired."));

            var hashedPassword = passwordHasher.HashPassword(null, newPassword);
            user.ResetPassword(hashedPassword);

            await userRepository.UpdateAsync(user);
            return Result.SuccessWithMessage("Password reset successfully.");
        }
    }
}