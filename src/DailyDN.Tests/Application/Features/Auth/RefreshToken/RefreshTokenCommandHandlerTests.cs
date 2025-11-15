using AutoMapper;
using DailyDN.Application.Features.Auth.RefreshToken;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Infrastructure.Models;
using FluentAssertions;
using Moq;

namespace DailyDN.Tests.Application.Features.Auth.RefreshToken
{
    [TestFixture]
    public class RefreshTokenCommandHandlerTests
    {
        private Mock<IAuthService> _authServiceMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private RefreshTokenCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthService>();
            _mapperMock = new Mock<IMapper>();
            _handler = new RefreshTokenCommandHandler(_authServiceMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenTokenIsRefreshed()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var tokenResponse = new TokenResponse("access-token", refreshToken, DateTime.UtcNow.AddMinutes(10), DateTime.UtcNow.AddHours(1));
            var command = new RefreshTokenCommand(refreshToken);
            var expectedResponse = new RefreshTokenCommandResponse(tokenResponse.AccessToken, tokenResponse.RefreshTokenHash, tokenResponse.RefreshTokenExpiration);

            _authServiceMock.Setup(s => s.RefreshTokenAsync(refreshToken))
                            .ReturnsAsync(tokenResponse);

            _mapperMock.Setup(m => m.Map<RefreshTokenCommandResponse>(tokenResponse))
                       .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(expectedResponse);
            result.Message.Should().Be("Token refreshed successfully.");
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenTokenIsInvalid()
        {
            // Arrange
            var refreshToken = "invalid-token";
            var command = new RefreshTokenCommand(refreshToken);

            _authServiceMock.Setup(s => s.RefreshTokenAsync(refreshToken))
                            .ReturnsAsync((TokenResponse?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("Otp.Invalid");
            result.Error.Message.Should().Be("OTP is invalid or has expired.");
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var refreshToken = "any-token";
            var command = new RefreshTokenCommand(refreshToken);

            _authServiceMock.Setup(s => s.RefreshTokenAsync(refreshToken))
                            .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("InvalidToken");
            result.Error.Message.Should().Be("Invalid or expired refresh token.");
        }
    }
}
