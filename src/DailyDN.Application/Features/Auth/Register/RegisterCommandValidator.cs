using FluentValidation;

namespace DailyDN.Application.Features.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MaximumLength(50).WithMessage("Surname cannot be longer than 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{10,15}$")
                .WithMessage("Please enter a valid phone number. It may start with + and contain 10 to 15 digits.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
            // .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            // .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
            // .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            // .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            // .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            // .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        }
    }
}
