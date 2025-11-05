using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace DailyDN.Application.Features.Posts.Add
{
    public class AddPostCommandHandler(
        IMapper mapper,
        IPostsService postsService,
        ILogger<AddPostCommandHandler> logger,
        IAuthenticatedUser authenticatedUser
    ) : ICommandHandler<AddPostCommand>
    {
        public async Task<Result> Handle(AddPostCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Adding Post by user: {@User}. Payload: {@Request}", authenticatedUser, request);
            var post = mapper.Map<Post>(request);
            await postsService.AddAsync(post, cancellationToken);
            return Result.SuccessWithMessage("Post added successfully.");
        }
    }
}