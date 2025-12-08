namespace DailyDN.Application.Dtos.RedisUser
{
    public class RedisUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string Email { get; set; } = null!;
        public ICollection<RedisUserRoleDto> UserRoles { get; set; } = [];
    }
}