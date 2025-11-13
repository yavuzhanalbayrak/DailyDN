using System;
using System.Threading.Tasks;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Contexts;
using DailyDN.Infrastructure.Helpers;
using DailyDN.Infrastructure.Models;
using DailyDN.Infrastructure.Services;
using DailyDN.Infrastructure.Services.Impl;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace DailyDN.Tests.Infrastructure.Services
{
    [TestFixture]
    public class TokenServiceTests
    {
        private TokenService _tokenService = null!;
        private Mock<IOptions<JwtSettings>> _jwtOptionsMock = null!;
        private DailyDNDbContext _context = null!;
        private readonly string userAgent = "unit-test-agent";
        private readonly string userIp = "127.0.0.1";


        [SetUp]
        public void Setup()
        {
            // In-memory DbContext
            var options = new DbContextOptionsBuilder<DailyDNDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DailyDNDbContext(options, Mock.Of<Microsoft.Extensions.Logging.ILogger<DailyDNDbContext>>(), Mock.Of<IAuthenticatedUser>());

            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            _jwtOptionsMock.Setup(x => x.Value).Returns(new JwtSettings
            {
                Key = "ThisIsASecretKeyForTesting1234567890",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiresInMinutes = 1440,
                RefreshTokenExpiresInDays = 7
            });

            _tokenService = new TokenService(_jwtOptionsMock.Object, _context);
        }

        [Test]
        public async Task GenerateTokens_ShouldReturnAccessAndRefreshToken()
        {
            // Arrange
            var user = new User("John", "Doe", "john@example.com", "hashedpassword");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var tokenResponse = await _tokenService.GenerateTokens(user.Id, userIp, userAgent);

            // Assert
            tokenResponse.Should().NotBeNull();
            tokenResponse.AccessToken.Should().NotBeNullOrWhiteSpace();
            tokenResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
            tokenResponse.AccessTokenExpiration.Should().BeAfter(DateTime.UtcNow);
            tokenResponse.RefreshTokenExpiration.Should().BeAfter(DateTime.UtcNow);

            var refreshTokenHash = HashHelper.HashSha256(tokenResponse.RefreshToken);

            var session = await _context.Set<UserSession>().FirstOrDefaultAsync();
            session.Should().NotBeNull();
            session!.UserId.Should().Be(user.Id);
            session!.RefreshToken.Should().Be(refreshTokenHash);
            session!.ExpiresAt.Should().Be(tokenResponse.RefreshTokenExpiration);
            session!.UserAgent.Should().Be(userAgent);
            session!.IpAddress.Should().Be(userIp);
            session.IsRevoked.Should().BeFalse();
        }

        [Test]
        public async Task RotateRefreshToken_ShouldRevokeOldTokenAndReturnNewAccessToken()
        {
            // Arrange
            var user = new User("Jane", "Smith", "jane@example.com", "hashedpassword");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var tokenResponse = await _tokenService.GenerateTokens(user.Id, userIp, userAgent);

            // Act
            var rotatedToken = await _tokenService.RotateRefreshToken(tokenResponse.RefreshToken, userIp, userAgent);

            // Assert
            rotatedToken.Should().NotBeNull();
            rotatedToken.AccessToken.Should().NotBeNullOrWhiteSpace();
            rotatedToken.RefreshToken.Should().NotBeNullOrWhiteSpace();
            rotatedToken.RefreshToken.Should().NotBe(tokenResponse.RefreshToken);
            rotatedToken.AccessTokenExpiration.Should().BeAfter(DateTime.UtcNow);
            rotatedToken.RefreshTokenExpiration.Should().BeAfter(DateTime.UtcNow);

            var sessions = await _context.Set<UserSession>().ToListAsync();
            sessions.Count.Should().Be(2);

            var oldRefreshTokenHash = HashHelper.HashSha256(tokenResponse.RefreshToken);
            var newRefreshTokenHash = HashHelper.HashSha256(rotatedToken.RefreshToken);

            var oldSession = sessions.Find(s => s.RefreshToken == oldRefreshTokenHash);
            var newSession = sessions.Find(s => s.RefreshToken == newRefreshTokenHash);
            oldSession?.IsRevoked.Should().Be(true);
            newSession.Should().NotBeNull();
            newSession!.UserId.Should().Be(user.Id);
            newSession!.RefreshToken.Should().Be(newRefreshTokenHash);
            newSession!.ExpiresAt.Should().Be(rotatedToken.RefreshTokenExpiration);
            newSession!.UserAgent.Should().Be(userAgent);
            newSession!.IpAddress.Should().Be(userIp);
            newSession.IsRevoked.Should().BeFalse();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

    }
}
