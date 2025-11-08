using FluentValidation;

namespace DailyDN.Application.Features.Auth.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Reset token cannot be empty.");

            RuleFor(x => x.NewPassword)
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