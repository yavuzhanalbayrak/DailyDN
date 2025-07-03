using MediatR;
using DailyDN.Application.Common.Model;

namespace DailyDN.Application.Messaging;

public interface IPaginatedQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, PaginatedResult<TResponse>>
where TQuery : IPaginatedQuery<TResponse> where TResponse : class, new()
{
}
