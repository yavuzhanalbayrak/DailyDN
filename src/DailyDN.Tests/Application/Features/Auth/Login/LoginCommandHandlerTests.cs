using DailyDN.Application.Common.Model;
using DailyDN.Application.Features.Auth.Login;
using DailyDN.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace DailyDN.Tests.Application.Features.Auth.Login
{
    [TestFixture]
    public class LoginCommandHandlerTests
    {
        private Mock<IAuthService> _authServiceMock = null!;
        private LoginCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthService>();
            _handler = new LoginCommandHandler(_authServiceMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenLoginIsSuccessful()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "password123");
            var expectedResult = Result.SuccessWithMessage("Login successful");

            _authServiceMock
                .Setup(s => s.LoginAsync(command.Email, command.Password))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Login successful");

            _authServiceMock.Verify(s => s.LoginAsync(command.Email, command.Password), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenLoginFails()
        {
            // Arrange
            var command = new LoginCommand("wrong@example.com", "wrongpassword");
            var expectedResult = Result.Failure(new Error("Auth.InvalidCredentials", "Invalid credentials."));

            _authServiceMock
                .Setup(s => s.LoginAsync(command.Email, command.Password))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("Auth.InvalidCredentials");
            result.Error.Message.Should().Be("Invalid credentials.");

            _authServiceMock.Verify(s => s.LoginAsync(command.Email, command.Password), Times.Once);
        }
    }

    internal static class LoggerMockExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, string containsMessage, Times times)
        {
            loggerMock.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(containsMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>() ),
                times);
        }
    }
}
