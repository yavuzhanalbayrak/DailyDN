using MediatR;
using DailyDN.Application.Common.Model;

namespace DailyDN.Application.Messaging
{
    public interface ICommand : IRequest<Result>
    {
    }
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
