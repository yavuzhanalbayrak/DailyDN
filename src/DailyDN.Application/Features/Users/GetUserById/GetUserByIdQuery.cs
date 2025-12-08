using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Users.GetUserById
{
    public record GetUserByIdQuery(int Id) : IQuery<GetUserByIdQueryResponse>;
}