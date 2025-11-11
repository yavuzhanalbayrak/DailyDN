using AutoMapper;
using DailyDN.Application.Features.Posts.Add;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DailyDN.Tests.Application.Features.Posts.Add
{
    [TestFixture]
    public class AddPostCommandHandlerTests
    {
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IPostsService> _postsServiceMock = null!;
        private Mock<ILogger<AddPostCommandHandler>> _loggerMock = null!;
        private Mock<IAuthenticatedUser> _authenticatedUserMock = null!;
        private AddPostCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _mapperMock = new Mock<IMapper>();
            _postsServiceMock = new Mock<IPostsService>();
            _loggerMock = new Mock<ILogger<AddPostCommandHandler>>();
            _authenticatedUserMock = new Mock<IAuthenticatedUser>();

            _handler = new AddPostCommandHandler(
                _mapperMock.Object,
                _postsServiceMock.Object,
                _loggerMock.Object,
                _authenticatedUserMock.Object
            );
        }

        [Test]
        public async Task Handle_Should_Add_Post_And_Return_SuccessResult()
        {
            // Arrange
            var command = new AddPostCommand
            {
                Caption = "Test Caption",
                MediaType = "Test MediaType",
                MediaUrl = "Test MediaUrl"
            };

            var post = new Post { Id = 1, Caption = command.Caption, MediaType = command.MediaType, MediaUrl = command.MediaUrl };

            _mapperMock.Setup(m => m.Map<Post>(command)).Returns(post);
            _postsServiceMock.Setup(s => s.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Post added successfully.");

            _mapperMock.Verify(m => m.Map<Post>(command), Times.Once);
            _postsServiceMock.Verify(s => s.AddAsync(post, It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Adding Post")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
