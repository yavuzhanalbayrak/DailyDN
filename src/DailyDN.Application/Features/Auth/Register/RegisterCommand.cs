using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.Register
{
    public record RegisterCommand(
        string Name,
        string Surname,
        string Email,
        string PhoneNumber,
        string Password
    ) : ICommand;
}