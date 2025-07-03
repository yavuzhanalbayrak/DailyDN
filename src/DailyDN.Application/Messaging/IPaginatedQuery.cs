using MediatR;
using DailyDN.Application.Common.Model;

namespace DailyDN.Application.Messaging;

public interface IPaginatedQuery<TResponse> : IRequest<PaginatedResult<TResponse>>, IPaginatedQuery where TResponse : class, new()
{
}

public interface IPaginatedQuery
{
    /// <summary>
    ///     Property to get and set the page number.
    /// </summary>
    int Page { get; set; }

    /// <summary>
    ///     Gets or sets the page size.
    /// </summary>
    int PageSize { get; set; }
}