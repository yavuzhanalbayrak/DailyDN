using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;
using DailyDN.Infrastructure.Services;
using AutoMapper;
using DailyDN.Application.Dtos.RedisUser;
using DailyDN.Application.Common.Model;
using Microsoft.AspNetCore.Http;

namespace DailyDN.Application.Services.Implementations
{
    public class UserService(
        IUnitOfWork uow,
        ILogger<UserService> logger,
        ICacheService redis,
        IMapper mapper,
        IAuthenticatedUser authenticatedUser,
        IFileStorageService fileStorageService
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

            var user = await uow.Users.GetUserWithRolesAsync(id);
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

        public async Task<Result<string>> UpdateProfilePhoto(IFormFile file)
        {
            var userId = authenticatedUser.UserId;

            if (userId == 0)
                return Result.Failure<string>(new("Unauthorized", "User not authenticated."));

            var user = await uow.Users.GetByIdAsync(userId);
            if (user is null)
                return Result.Failure<string>(new("NotFound", "User not found."));

            var photoUrl = await fileStorageService.SaveProfilePhotoAsync(userId.ToString(), file);

            user.SetAvatar(photoUrl);

            await uow.Users.UpdateAsync(user);
            await uow.SaveChangesAsync();

            return Result.Success(photoUrl);
        }
    }
}