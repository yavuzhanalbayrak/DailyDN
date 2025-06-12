using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Repositories;

namespace DailyDN.Application.Features.Auth.Login
{
    public class LoginCommandHandler(IGenericRepository<User> genericRepository) : ICommandHandler<LoginCommand>
    {
        public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await genericRepository.GetAsync(u => u.Email == request.Email);
            if (user.Any())
            {
                return Result.Success(new { Guid = "asd...", OTP = 123456 });
            }
            else
                return Result.Failure(new Error("Unauthorized", "The email address you entered is incorrect or not registered."));
        }
    }
}