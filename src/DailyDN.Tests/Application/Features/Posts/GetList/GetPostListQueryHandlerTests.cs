using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Features.Posts.GetList;
using DailyDN.Application.Features.Posts.GetList.Response;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DailyDN.Tests.Application.Features.Posts.GetList
{
    [TestFixture]
    public class GetPostListQueryHandlerTests
    {
        private Mock<IPostsService> _postsServiceMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<ILogger<GetPostListQueryHandler>> _loggerMock = null!;
        private Mock<IAuthenticatedUser> _authenticatedUserMock = null!;
        private GetPostListQueryHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _postsServiceMock = new Mock<IPostsService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<GetPostListQueryHandler>>();
            _authenticatedUserMock = new Mock<IAuthenticatedUser>();

            _handler = new GetPostListQueryHandler(
                _postsServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _authenticatedUserMock.Object
            );
        }

        [Test]
        public async Task Handle_Should_Return_PaginatedResult_With_Posts()
        {
            // Arrange
            var query = new GetPostListQuery { Page = 1, PageSize = 2 };

            var posts = new List<Post>
            {
                new() { Id = 1, Caption = "Post 1", MediaUrl = "url1", MediaType = "image" },
                new() { Id = 2, Caption = "Post 2", MediaUrl = "url2", MediaType = "video" }
            };
            var totalCount = 2;

            var mappedResponse = new GetPostListQueryResponse
            {
                Posts =
                [
                    new() { Id = 1, Caption = "Post 1", MediaUrl = "url1", MediaType = "image" },
                    new() { Id = 2, Caption = "Post 2", MediaUrl = "url2", MediaType = "video" }
                ]
            };

            _postsServiceMock.Setup(s => s.GetListAsync(query.Page, query.PageSize))
                             .ReturnsAsync((posts, totalCount));

            _mapperMock.Setup(m => m.Map<GetPostListQueryResponse>(posts))
                       .Returns(mappedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PaginatedResult<GetPostListQueryResponse>>();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(mappedResponse);
            result.TotalItems.Should().Be(totalCount);
            result.CurrentPage.Should().Be(query.Page);
            result.PageSize.Should().Be(query.PageSize);

            _postsServiceMock.Verify(s => s.GetListAsync(query.Page, query.PageSize), Times.Once);
            _mapperMock.Verify(m => m.Map<GetPostListQueryResponse>(posts), Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Getting Posts")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
