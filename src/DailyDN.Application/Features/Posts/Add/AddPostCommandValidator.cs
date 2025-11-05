using FluentValidation;

namespace DailyDN.Application.Features.Posts.Add
{
    public class AddPostCommandValidator : AbstractValidator<AddPostCommand>
    {
        public AddPostCommandValidator()
        {
        }
    }
}