
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.Login
{
    public record LoginCommand(string Email, string Password) : ICommand;
}