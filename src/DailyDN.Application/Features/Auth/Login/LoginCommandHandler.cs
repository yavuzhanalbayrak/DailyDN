using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DailyDN.Application.Features.Auth.Login
{
    public class LoginCommandHandler(IGenericRepository<User> userRepository, IPasswordHasher<User> passwordHasher) : ICommandHandler<LoginCommand>
    {
        public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(u => u.Email == request.Email);
            if (!user.Any())
            {
                return Result.Failure(new Error("Unauthorized", "Email or password is incorrect."));
            }

            var userEntity = user[0];
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(userEntity, userEntity.PasswordHash, request.Password);

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
    }
}