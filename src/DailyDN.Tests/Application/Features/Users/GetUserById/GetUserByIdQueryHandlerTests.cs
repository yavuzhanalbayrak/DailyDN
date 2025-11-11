using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Features.Users.GetUserById;
using DailyDN.Application.Features.Users.GetUserById.Responses;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DailyDN.Tests.Application.Features.Users.GetUserById
{
    [TestFixture]
    public class GetUserByIdQueryHandlerTests
    {
        private Mock<ILogger<GetUserByIdQueryHandler>> _loggerMock = null!;
        private Mock<IUserService> _userServiceMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private GetUserByIdQueryHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<GetUserByIdQueryHandler>>();
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetUserByIdQueryHandler(
                _loggerMock.Object,
                _userServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task Handle_Should_Return_Success_When_User_Found()
        {
            // Arrange
            var query = new GetUserByIdQuery(1);

            var user = new User
            (
                "John",
                "Doe",
                "john@example.com",
                "passwordHash",
                "/url",
                1
            );

            var expectedResponse = new GetUserByIdQueryResponse
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com"
            };

            _userServiceMock.Setup(s => s.GetByIdAsync(query.Id))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<GetUserByIdQueryResponse>(user))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(expectedResponse);
            result.Message.Should().Be("User details retrieved successfully.");

            _userServiceMock.Verify(s => s.GetByIdAsync(query.Id), Times.Once);
            _mapperMock.Verify(m => m.Map<GetUserByIdQueryResponse>(user), Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Fetching user details")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Handle_Should_Return_Failure_When_User_Not_Found()
        {
            // Arrange
            var query = new GetUserByIdQuery(99);

            _userServiceMock.Setup(s => s.GetByIdAsync(query.Id))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("User.NotFound");
            result.Error.Message.Should().Contain(query.Id.ToString());

            _userServiceMock.Verify(s => s.GetByIdAsync(query.Id), Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("User not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );

            _mapperMock.Verify(m => m.Map<GetUserByIdQueryResponse>(It.IsAny<User>()), Times.Never);
        }
    }
}
