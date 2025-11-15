using DailyDN.Application.Common.Model;
using DailyDN.Application.Features.Auth.Register;
using DailyDN.Application.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace DailyDN.Tests.Application.Features.Auth.Register
{
    [TestFixture]
    public class RegisterCommandHandlerTests
    {
        private Mock<IAuthService> _authServiceMock = null!;
        private Mock<ILogger<RegisterCommandHandler>> _loggerMock = null!;
        private RegisterCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<RegisterCommandHandler>>();
            _handler = new RegisterCommandHandler(_authServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenRegistrationSucceeds()
        {
            // Arrange
            var command = new RegisterCommand("John", "Doe", "john.doe@example.com", "5002001020", "StrongPass1!");
            var expectedResult = Result.SuccessWithMessage("User registered successfully.");

            _authServiceMock
                .Setup(s => s.RegisterAsync(
                    command.Name,
                    command.Surname,
                    command.Email,
                    command.PhoneNumber,
                    command.Password,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("User registered successfully.");

            _authServiceMock.Verify(s => s.RegisterAsync(
                command.Name,
                command.Surname,
                command.Email,
                command.PhoneNumber,
                command.Password,
                It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Information, $"User registering. Email: {command.Email}", Times.Once());
        }
        [Test]
        public async Task Handle_ShouldReturnFailure_WhenRegistrationFails()
        {
            // Arrange
            var error = new Error("Auth.EmailOrPhoneNumberTaken", "Email or phone number is already taken.");
            var command = new RegisterCommand("John", "Doe", "john.doe@example.com", "5002001020", "WeakPass");
            var expectedResult = Result.Failure(error);

            _authServiceMock
                .Setup(s => s.RegisterAsync(
                    command.Name,
                    command.Surname,
                    command.Email,
                    command.PhoneNumber,
                    command.Password,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be(error.Code);
            result.Error.Message.Should().Be(error.Message);

            _authServiceMock.Verify(s => s.RegisterAsync(
                command.Name,
                command.Surname,
                command.Email,
                command.PhoneNumber,
                command.Password,
                It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Information, $"User registering. Email: {command.Email}", Times.Once());
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
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }
    }
}
