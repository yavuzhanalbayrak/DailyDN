using MediatR;
using DailyDN.Application.Common.Model;

namespace DailyDN.Application.Messaging;


public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}

