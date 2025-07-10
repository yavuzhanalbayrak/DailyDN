using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Repositories;

namespace DailyDN.Application.Features.Posts.Add
{
    public class AddPostCommandHandler(IGenericRepository<Post> postRepository, IMapper mapper) : ICommandHandler<AddPostCommand>
    {
        public async Task<Result> Handle(AddPostCommand request, CancellationToken cancellationToken)
        {
            var post = mapper.Map<Post>(request);
            await postRepository.AddAsync(post, cancellationToken);
            return Result.SuccessWithMessage("Post added successfully.");
        }
    }
}