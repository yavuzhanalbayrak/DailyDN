using DailyDN.Application.Features.Users.GetUserById.Responses;
using DailyDN.Domain.ValueObjects;

namespace DailyDN.Application.Features.Users.GetUserById
{
    public class GetUserByIdQueryResponse
    {
        public int Id { get; set; }
        public FullName FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public ICollection<UserRoleResponse> UserRoles { get; set; } = [];
    }
}