using DailyDN.Application.Features.Auth.Register;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace DailyDN.Tests.Application.Features.Auth.Register
{
    [TestFixture]
    public class RegisterCommandValidatorTests
    {
        private RegisterCommandValidator _validator = null!;

        [SetUp]
        public void SetUp()
        {
            _validator = new RegisterCommandValidator();
        }

        [Test]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var command = new RegisterCommand("", "Doe", "john@example.com", "5002001020", "StrongPass1!");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Test]
        public void Should_Have_Error_When_Surname_Is_Empty()
        {
            var command = new RegisterCommand("John", "", "john@example.com", "5002001020", "StrongPass1!");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Surname);
        }

        [Test]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new RegisterCommand("John", "Doe", "invalid-email", "5002001020", "StrongPass1!");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void Should_Have_Error_When_Password_Is_Weak()
        {
            var command = new RegisterCommand("John", "Doe", "john@example.com", "5002001020", "");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
        [Test]
        public void Should_Have_Error_When_PhoneNumber_Is_Invalid()
        {
            var command = new RegisterCommand("John", "Doe", "john@example.com", "invalid-phone", "StrongPass1!");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Test]
        public void Should_Not_Have_Error_For_Valid_Command()
        {
            var command = new RegisterCommand("John", "Doe", "john@example.com", "5002001020", "StrongPass1!");
            var result = _validator.TestValidate(command);
            result.IsValid.Should().BeTrue();
        }
    }
}
