using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using DailyDN.Infrastructure.Services;
using AutoMapper;
using DailyDN.Application.Dtos.RedisUser;

namespace DailyDN.Application.Services.Implementations
{
    public class UserService(
        IUserRepository userRepository,
        ILogger<UserService> logger,
        ICacheService redis,
        IMapper mapper
    ) : IUserService
    {
        private const string CacheKeyPrefix = "user:";

        public async Task<User?> GetByIdAsync(int id)
        {
            string cacheKey = $"{CacheKeyPrefix}{id}";

            var cachedUser = await redis.GetAsync<RedisUserDto>(cacheKey);
            if (cachedUser is not null)
            {
                var response = mapper.Map<User>(cachedUser);

                return response;
            }

            logger.LogInformation("User {UserId} not found in cache, querying database", id);

            var user = await userRepository.GetUserWithRolesAsync(id);
            if (user == null)
            {
                logger.LogWarning("User {UserId} not found in database", id);
                return null;
            }

            var redisUser = mapper.Map<RedisUserDto>(user);
            await redis.SetAsync(
                cacheKey,
                redisUser,
                TimeSpan.FromMinutes(30)
            );

            return user;
        }
    }
}