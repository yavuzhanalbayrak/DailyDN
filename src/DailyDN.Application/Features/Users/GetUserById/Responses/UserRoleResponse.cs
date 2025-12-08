namespace DailyDN.Application.Features.Users.GetUserById.Responses
{
    public class UserRoleResponse
    {
        public int Id { get; set; }
        public string Name { get; private set; } = null!;
    }
}