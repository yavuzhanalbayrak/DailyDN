using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Domain.Entities;

namespace DailyDN.Application.Features.Posts.Add
{
    public class AddPostCommandHandler(
        IMapper mapper,
        IPostsService postsService
    ) : ICommandHandler<AddPostCommand>
    {
        public async Task<Result> Handle(AddPostCommand request, CancellationToken cancellationToken)
        {
            var post = mapper.Map<Post>(request);
            await postsService.AddAsync(post, cancellationToken);
            return Result.SuccessWithMessage("Post added successfully.");
        }
    }
}