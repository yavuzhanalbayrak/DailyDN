using DailyDN.Application.Features.Users.GetUserById.Responses;

namespace DailyDN.Application.Features.Users.GetUserById
{
    public class GetUserByIdQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string Email { get; set; } = null!;
        public ICollection<UserRoleResponse> UserRoles { get; set; } = [];
    }
}