using MediatR;
using DailyDN.Application.Common.Model;

namespace DailyDN.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
