using DailyDN.Application.Common.Attributes;
using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.Register
{
    public class RegisterCommand(
        string name,
        string surname,
        string email,
        string phoneNumber,
        string password
    ) : ICommand
    {
        public string Name { get; set; } = name;
        public string Surname { get; set; } = surname;
        public string Email { get; set; } = email;
        [DoNotLog]
        public string PhoneNumber { get; set; } = phoneNumber;
        [DoNotLog]
        public string Password { get; set; } = password;
    }
}