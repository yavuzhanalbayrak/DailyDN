using DailyDN.Application.Common.Model;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DailyDN.Application.Services.Implementations
{
    public class AuthService(
        IGenericRepository<User> userRepository,
        IPasswordHasher<User> passwordHasher
    ) : IAuthService
    {
        public Task LoginAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Result> RegisterAsync(
            string Name,
            string Surname,
            string Email,
            string Password,
            CancellationToken cancellationToken
        )
        {
            var exists = await userRepository.GetAsync(u => u.Email == Email);
            if (exists.Any())
            {
                return Result.Failure(new Error("Conflict", "This email is already registered."));
            }

            var hashedPassword = passwordHasher.HashPassword(null, Password);

            var user = new User(
                name: Name,
                surname: Surname,
                email: Email,
                passwordHash: hashedPassword
            );

            //TODO: email kontrolü için ayrı bir endpoint yazılacak.
            user.MarkEmailVerified();

            await userRepository.AddAsync(user, cancellationToken);

            return Result.SuccessWithMessage("Registration completed successfully.");
        }

        public Task VerifyOtpAsync()
        {
            throw new NotImplementedException();
        }
    }
}