using DailyDN.Application.Messaging;
using Microsoft.AspNetCore.Http;

namespace DailyDN.Application.Features.Users.UpdateProfilePhoto
{
    public record UpdateProfilePhotoCommand(IFormFile File) : ICommand;
}