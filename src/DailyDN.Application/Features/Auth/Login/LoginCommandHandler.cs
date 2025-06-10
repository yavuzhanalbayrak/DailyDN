using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand>
    {
        public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return Result.Success(new { Guid = "asd...", OTP = 123456 });
        }
    }
}