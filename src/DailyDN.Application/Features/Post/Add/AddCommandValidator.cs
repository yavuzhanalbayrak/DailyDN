using FluentValidation;

namespace DailyDN.Application.Features.Post.Add
{
    public class AddCommandValidator : AbstractValidator<AddCommand>
    {
        public AddCommandValidator()
        {
        }
    }
}