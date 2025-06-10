using FluentValidation;
using FluentValidation.Results;
using MediatR;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace DailyDN.Application.Behaviors
{
    /// <summary>
    ///     Constructor for ValidationBehavior class.
    /// </summary>
    /// <param name="validators">Collection of validators.</param>
    /// <returns>
    ///     An instance of ValidationBehavior class.
    /// </returns>
    public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> _validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        /// <summary>
        ///     Handles a request by validating it and then passing it to the next handler.
        /// </summary>
        /// <param name="request">The request to be handled.</param>
        /// <param name="next">The next handler in the pipeline.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response from the next handler in the pipeline.</returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            ValidationResult[] validationResults =
                await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            List<ValidationFailure> failures = [.. validationResults.SelectMany(    r => r.Errors).Where(f => f != null)];

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return await next(cancellationToken);
        }
    }
}
