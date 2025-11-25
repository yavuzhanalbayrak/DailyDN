using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;

namespace DailyDN.Application.Features.Users.UpdateProfilePhoto
{
    public class UpdateProfilePhotoCommandHandler(IUserService userService) : ICommandHandler<UpdateProfilePhotoCommand>
    {
        public async Task<Result> Handle(UpdateProfilePhotoCommand request, CancellationToken cancellationToken)
        {
            return await userService.UpdateProfilePhoto(request.File);
        }
    }
}