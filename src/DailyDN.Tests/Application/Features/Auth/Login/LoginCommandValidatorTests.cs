using DailyDN.Application.Features.Auth.Login;
using FluentValidation.TestHelper;

namespace DailyDN.Tests.Application.Features.Auth.Login
{
    [TestFixture]
    public class LoginCommandValidatorTests
    {
        private LoginCommandValidator _validator = null!;

        [SetUp]
        public void SetUp()
        {
            _validator = new LoginCommandValidator();
        }

        [Test]
        public void Should_HaveError_WhenEmailIsEmpty()
        {
            // Arrange
            var command = new LoginCommand(string.Empty, "password123");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Email is required.");
        }

        [Test]
        public void Should_HaveError_WhenEmailIsInvalid()
        {
            // Arrange
            var command = new LoginCommand("invalidemail", "password123");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Please enter a valid email address.");
        }

        [Test]
        public void Should_HaveError_WhenPasswordIsEmpty()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", string.Empty);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password is required.");
        }

        [Test]
        public void Should_HaveError_WhenPasswordTooShort()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "123");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password must be at least 6 characters.");
        }

        [Test]
        public void Should_NotHaveAnyErrors_WhenCommandIsValid()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "password123");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
