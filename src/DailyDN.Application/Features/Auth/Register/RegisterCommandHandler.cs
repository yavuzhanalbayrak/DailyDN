using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DailyDN.Application.Features.Auth.Register
{
    public class RegisterCommandHandler(IGenericRepository<User> userRepository, IPasswordHasher<User> passwordHasher)
        : ICommandHandler<RegisterCommand>
    {
        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var exists = await userRepository.GetAsync(u => u.Email == request.Email);
            if (exists.Any())
            {
                return Result.Failure(new Error("Conflict", "This email is already registered."));
            }

            var hashedPassword = passwordHasher.HashPassword(null, request.Password);

            var user = new User(
                name: request.Name,
                surname: request.Surname,
                email: request.Email,
                passwordHash: hashedPassword
            );

            //TODO: email kontrolü için ayrı bir endpoint yazılacak.
            user.MarkEmailVerified();

            await userRepository.AddAsync(user, cancellationToken);

            return Result.SuccessWithMessage("Registration completed successfully.");
        }
    }

}