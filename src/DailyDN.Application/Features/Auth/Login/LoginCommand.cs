
using DailyDN.Application.Common.Attributes;
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.Login
{
    public class LoginCommand(string email, string password) : ICommand
    {
        public string Email { get; set; } = email;
        [DoNotLog]
        public string Password { get; set; } = password;
    }
}