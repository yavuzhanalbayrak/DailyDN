using DailyDN.Application.Common.Model;
using DailyDN.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DailyDN.Application.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> GetByIdAsync(int id);
        public Task<Result<string>> UpdateProfilePhoto(IFormFile file);

    }
}