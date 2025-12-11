using AutoMapper;
using DailyDN.Application.Features.Auth.VerifyOtp;
using DailyDN.Application.Services.Interfaces;
using Moq;
using FluentAssertions;
using DailyDN.Infrastructure.Models;

namespace DailyDN.Tests.Application.Features.Auth.VerifyOtp
{
    [TestFixture]
    public class VerifyOtpCommandHandlerTests
    {
        private Mock<IOtpService> _otpServiceMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private VerifyOtpCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _otpServiceMock = new Mock<IOtpService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new VerifyOtpCommandHandler(
                _mapperMock.Object,
                _otpServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenOtpIsValid()
        {
            // Arrange
            var command = new VerifyOtpCommand(Guid.NewGuid(), "123456");

            var fakeToken = new TokenResponse(
                "access-token",
                "refresh-token",
                DateTime.UtcNow.AddMinutes(30),
                DateTime.UtcNow.AddDays(7)
            );

            var response = new VerifyOtpCommandResponse
            {
                AccessToken = fakeToken.AccessToken,
                AccessTokenExpiration = fakeToken.AccessTokenExpiration,
                RefreshTokenHash = fakeToken.RefreshTokenHash,
                RefreshTokenExpiration = fakeToken.RefreshTokenExpiration
            };

            _otpServiceMock
                .Setup(x => x.VerifyOtpAsync(command.Guid, command.Otp))
                .ReturnsAsync(fakeToken);

            _mapperMock
                .Setup(x => x.Map<VerifyOtpCommandResponse>(fakeToken))
                .Returns(response);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(response);

            _otpServiceMock.Verify(x => x.VerifyOtpAsync(command.Guid, command.Otp), Times.Once);
            _mapperMock.Verify(x => x.Map<VerifyOtpCommandResponse>(fakeToken), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenOtpIsInvalid()
        {
            // Arrange
            var command = new VerifyOtpCommand(Guid.NewGuid(), "wrong-otp");

            _otpServiceMock
                .Setup(x => x.VerifyOtpAsync(command.Guid, command.Otp))
                .ReturnsAsync((TokenResponse?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("Otp.Invalid");
            result.Error.Message.Should().Be("OTP is invalid or has expired.");

            _otpServiceMock.Verify(x => x.VerifyOtpAsync(command.Guid, command.Otp), Times.Once);
            _mapperMock.Verify(x => x.Map<VerifyOtpCommandResponse>(It.IsAny<object>()), Times.Never);
        }
    }
}
