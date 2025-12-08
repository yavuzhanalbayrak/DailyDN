using DailyDN.Application.Features.Auth.ForgotPassword;
using DailyDN.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace DailyDN.Tests.Application.Features.Auth.ForgotPassword
{
    [TestFixture]
    public class ForgotPasswordCommandHandlerTests
    {
        private Mock<IAuthService> _authServiceMock = null!;
        private ForgotPasswordCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthService>();

            _handler = new ForgotPasswordCommandHandler(
                _authServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldCallAuthServiceAndReturnSuccess()
        {
            // Arrange
            var command = new ForgotPasswordCommand("test@example.com");

            _authServiceMock
                .Setup(x => x.ForgotPasswordAsync(command.Email))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("If an account with this email exists, a password reset link has been sent.");

            _authServiceMock.Verify(x => x.ForgotPasswordAsync(command.Email), Times.Once);
        }
    }
}
