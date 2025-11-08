using DailyDN.Domain.Entities;

namespace DailyDN.Application.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> GetByIdAsync(int id);

    }
}