using DailyDN.Application.Features.Auth.ForgotPassword;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace DailyDN.Tests.Application.Features.Auth.ForgotPassword
{
    [TestFixture]
    public class ForgotPasswordCommandValidatorTests
    {
        private ForgotPasswordCommandValidator _validator = null!;

        [SetUp]
        public void SetUp()
        {
            _validator = new ForgotPasswordCommandValidator();
        }

        [Test]
        public void Should_Have_Error_When_Email_Is_Null_Or_Empty()
        {
            var command = new ForgotPasswordCommand("");
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email is required.");
        }

        [Test]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new ForgotPasswordCommand("invalid-email");
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Please enter a valid email address.");
        }

        [Test]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            var command = new ForgotPasswordCommand("test@example.com");
            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
