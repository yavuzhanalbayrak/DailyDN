using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DailyDN.Application.Features.Users.GetUserById
{
    public class GetUserByIdQueryHandler(ILogger<GetUserByIdQueryHandler> logger, IUserService userService, IMapper mapper) : IQueryHandler<GetUserByIdQuery, GetUserByIdQueryResponse>
    {
        public async Task<Result<GetUserByIdQueryResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching user details. UserId: {UserId}", request.Id);
            var user = await userService.GetByIdAsync(request.Id);
            if (user == null)
            {
                logger.LogWarning("User not found. UserId: {UserId}", request.Id);
                return Result.Failure<GetUserByIdQueryResponse>(new Error("User.NotFound",$"User with ID {request.Id} was not found."));
            }
            var response = mapper.Map<GetUserByIdQueryResponse>(user);

            return Result.SuccessWithMessage(response, "User details retrieved successfully.");
        }
    }
}