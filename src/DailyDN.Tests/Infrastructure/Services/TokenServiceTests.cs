using System.Linq.Expressions;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Helpers;
using DailyDN.Infrastructure.Models;
using DailyDN.Infrastructure.Repositories;
using DailyDN.Infrastructure.Services.Impl;
using DailyDN.Infrastructure.UnitOfWork;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace DailyDN.Tests.Infrastructure.Services
{
    [TestFixture]
    public class TokenServiceTests
    {
        private TokenService _tokenService = null!;
        private Mock<IOptions<JwtSettings>> _jwtOptionsMock = null!;
        private Mock<IUnitOfWork> _uowMock = null!;
        private Mock<IUserRepository> _userRepoMock = null!;
        private Mock<IUserSessionRepository> _sessionRepoMock = null!;

        private readonly string userAgent = "unit-test-agent";
        private readonly string userIp = "127.0.0.1";

        private User _fakeUser = null!;

        [SetUp]
        public void Setup()
        {
            // Fake user
            _fakeUser = new User("John", "Doe", "john@example.com", "hashed")
            {
                Id = 1,
                UserRoles =
                {
                    new UserRole
                    {
                        Role = new Role("Admin")
                        {
                            RoleClaims =
                            {
                                new RoleClaim()
                                {
                                    RoleId = 1,
                                    Role = new Role("Admin")
                                    {
                                        Id = 1
                                    },
                                    ClaimId = 1,
                                    Claim = new Claim("Role","Admin")
                                }
                            }
                        }
                    }
                }
            };

            // Mock Repositories
            _userRepoMock = new Mock<IUserRepository>();
            _sessionRepoMock = new Mock<IUserSessionRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            // Wiring repos into UoW
            _uowMock.SetupGet(x => x.Users).Returns(_userRepoMock.Object);
            _uowMock.SetupGet(x => x.UserSessions).Returns(_sessionRepoMock.Object);

            // Jwt settings
            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            _jwtOptionsMock.Setup(x => x.Value).Returns(new JwtSettings
            {
                Key = "ThisIsASecretKeyForTesting1234567890",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiresInMinutes = 1440,
                RefreshTokenExpiresInDays = 7
            });

            _tokenService = new TokenService(_jwtOptionsMock.Object, _uowMock.Object);

            // Default mocks
            _userRepoMock.Setup(x => x.GetUserWithRolesAndClaimsAsync(It.IsAny<int>()))
                         .ReturnsAsync(_fakeUser);

            _uowMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        }

        [Test]
        public async Task GenerateTokens_ShouldCreateSession_AndReturnTokens()
        {
            // Arrange
            UserSession? createdSession = null;

            _sessionRepoMock
                .Setup(x => x.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                .Callback<UserSession, CancellationToken>((s, _) => createdSession = s)
                .ReturnsAsync((UserSession s, CancellationToken _) => s);

            // Act
            var response = await _tokenService.GenerateTokens(_fakeUser.Id, userIp, userAgent);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().NotBeNullOrWhiteSpace();
            response.RefreshToken.Should().NotBeNullOrWhiteSpace();

            createdSession.Should().NotBeNull();
            createdSession!.UserId.Should().Be(_fakeUser.Id);

            var expectedHash = HashHelper.HashSha256(response.RefreshToken);
            createdSession.RefreshToken.Should().Be(expectedHash);
            createdSession.IpAddress.Should().Be(userIp);
            createdSession.UserAgent.Should().Be(userAgent);

            _uowMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RotateRefreshToken_ShouldRevokeOldToken_AndCreateNewOne()
        {
            // Arrange – old session
            string oldRefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            string oldHash = HashHelper.HashSha256(oldRefreshToken);

            var existingSession = new UserSession(
                _fakeUser.Id,
                oldHash,
                userIp,
                userAgent,
                DateTime.UtcNow.AddDays(7)
            );

            _sessionRepoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<UserSession, bool>>>()))
                .ReturnsAsync(existingSession);

            UserSession? newCreatedSession = null;
            _sessionRepoMock
                .Setup(x => x.AddAsync(It.IsAny<UserSession>(), default))
                .Callback<UserSession, CancellationToken>((s, _) => newCreatedSession = s)
                .ReturnsAsync(existingSession);


            // Act
            var newToken = await _tokenService.RotateRefreshToken(oldRefreshToken, userIp, userAgent);

            // Assert
            newToken.RefreshToken.Should().NotBe(oldRefreshToken);

            // old session must be revoked
            existingSession.IsRevoked.Should().BeTrue();

            newCreatedSession.Should().NotBeNull();
            newCreatedSession!.IsRevoked.Should().BeFalse();

            // new token session must match hashed refresh token
            var newHash = HashHelper.HashSha256(newToken.RefreshToken);
            newCreatedSession.RefreshToken.Should().Be(newHash);

            _uowMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
        }
    }
}
