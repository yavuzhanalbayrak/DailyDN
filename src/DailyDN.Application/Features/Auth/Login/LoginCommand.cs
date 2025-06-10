
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.Login
{
    public record LoginCommand(string UserName, string Password) : ICommand;
}