using DailyDN.Application.Features.Auth.ResetPassword;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace DailyDN.Tests.Application.Features.Auth.ResetPassword
{


    [TestFixture]
    public class ResetPasswordCommandValidatorTests
    {
        private ResetPasswordCommandValidator _validator = null!;

        [SetUp]
        public void SetUp()
        {
            _validator = new ResetPasswordCommandValidator();
        }

        [Test]
        public void Validator_Should_HaveError_WhenTokenIsEmpty()
        {
            var command = new ResetPasswordCommand(Guid.Empty, "StrongPass1!");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Token);
        }

        [Test]
        public void Validator_Should_HaveError_WhenPasswordDoesNotMeetRequirements()
        {
            var command = new ResetPasswordCommand(Guid.NewGuid(), "");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
        }

        [Test]
        public void Validator_Should_Pass_WhenAllRequirementsMet()
        {
            var command = new ResetPasswordCommand(Guid.NewGuid(), "StrongPass1!");
            var result = _validator.TestValidate(command);
            result.IsValid.Should().BeTrue();
        }
    }
}
