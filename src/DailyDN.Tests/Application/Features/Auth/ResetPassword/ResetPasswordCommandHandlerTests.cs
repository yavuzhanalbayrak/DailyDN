using DailyDN.Application.Common.Model;
using DailyDN.Application.Features.Auth.ResetPassword;
using DailyDN.Application.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DailyDN.Tests.Application.Features.Auth.ResetPassword
{
    [TestFixture]
    public class ResetPasswordCommandHandlerTests
    {
        private Mock<IAuthService> _authServiceMock = null!;
        private Mock<ILogger<ResetPasswordCommandHandler>> _loggerMock = null!;
        private ResetPasswordCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<ResetPasswordCommandHandler>>();
            _handler = new ResetPasswordCommandHandler(_authServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenResetPasswordSucceeds()
        {
            // Arrange
            var command = new ResetPasswordCommand
            (
                Guid.NewGuid(),
                "NewPassword123"
            );

            var expectedResult = Result.SuccessWithMessage("Password reset successfully.");

            _authServiceMock
                .Setup(s => s.ResetPasswordAsync(command.Token, command.NewPassword))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Password reset successfully.");

            _authServiceMock.Verify(s => s.ResetPasswordAsync(command.Token, command.NewPassword), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenResetPasswordFails()
        {
            // Arrange
            var command = new ResetPasswordCommand
            (

                Guid.NewGuid(),
                "NewPassword123"
            );

            var expectedResult = Result.Failure(new Error("Auth.InvalidToken", "Token is invalid."));

            _authServiceMock
                .Setup(s => s.ResetPasswordAsync(command.Token, command.NewPassword))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("Auth.InvalidToken");
            result.Error.Message.Should().Be("Token is invalid.");

            _authServiceMock.Verify(s => s.ResetPasswordAsync(command.Token, command.NewPassword), Times.Once);
        }
    }
}
