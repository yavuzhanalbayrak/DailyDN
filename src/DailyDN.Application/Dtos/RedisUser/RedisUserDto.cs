using DailyDN.Domain.ValueObjects;

namespace DailyDN.Application.Dtos.RedisUser
{
    public class RedisUserDto
    {
        public int Id { get; set; }
        public FullName FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public Email Email { get; set; } = null!;
        public ICollection<RedisUserRoleDto> UserRoles { get; set; } = [];
    }
}