namespace DailyDN.Application.Common.Model
{
    public class Result
    {
        public Result()
        {

        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Result" /> class with the specified parameters.
        /// </summary>
        /// <param name="isSuccess">The flag indicating if the result is successful.</param>
        /// <param name="error">The error that occurred.</param>
        protected Result(bool isSuccess, Error? error)
        {
            switch (isSuccess)
            {
                case true when error != null:
                    throw new InvalidOperationException();
                case false when error == null:
                    throw new InvalidOperationException();
                default:
                    IsSuccess = isSuccess;
                    Error = error;
                    break;
            }
        }
        protected Result(bool isSuccess, string message) : this(isSuccess, null)
        {
            if (isSuccess)
            {
                Message = message;
            }
        }

        /// <summary>
        ///     Gets the error.
        /// </summary>
        public Error? Error { get; }

        /// <summary>
        ///     Gets a value indicating whether the result is a success.
        /// </summary>
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        /// <summary>
        ///     Returns a success <see cref="Result" />.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>A new instance of <see cref="Result" />.</returns>
        public static Result Create(bool condition) => condition ? Success() : Failure(Error.ConditionNotMet);

        /// <summary>
        ///     Creates a new <see cref="Result{TValue}" /> with the specified nullable value and the specified error.
        /// </summary>
        /// <typeparam name="TValue">The result type.</typeparam>
        /// <param name="value">The result value.</param>
        /// <returns>A new instance of <see cref="Result{TValue}" /> with the specified value or an error.</returns>
        public static Result<TValue> Create<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

        /// <summary>
        ///     Returns a failure <see cref="Result" /> with the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>A new instance of <see cref="Result" /> with the specified error.</returns>
        public static Result Failure(Error error) => new(false, error);

        /// <summary>
        ///     Returns a failure <see cref="Result{TValue}" /> with the specified error.
        /// </summary>
        /// <typeparam name="TValue">The result type.</typeparam>
        /// <param name="error">The error.</param>
        /// <returns>A new instance of <see cref="Result{TValue}" /> with the specified error and failure flag set.</returns>
        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

        /// <summary>
        ///     Returns a failure <see cref="Result{TValue}" /> with the specified error.
        /// </summary>
        /// <typeparam name="TValue">The result type.</typeparam>
        /// <param name="error">The error.</param>
        /// <param name="value">The result value.</param>
        /// <returns>A new instance of <see cref="Result{TValue}" /> with the specified error and failure flag set.</returns>
        public static Result<TValue> Failure<TValue>(Error error, TValue value) => new(value, false, error);

        /// <summary>
        ///     Returns the first failure from the specified <paramref name="results" />.
        ///     If there is no failure, a success is returned.
        /// </summary>
        /// <param name="results">The results array.</param>
        /// <returns>
        ///     The first failure from the specified <paramref name="results" /> array,or a success if it does not exist.
        /// </returns>
        public static async Task<Result> FirstFailureOrSuccess(params Func<Task<Result>>[] results)
        {
            foreach (Func<Task<Result>> resultTask in results)
            {
                Result result = await resultTask();

                if (!result.IsSuccess)
                {
                    return result;
                }
            }

            return Success();
        }

        /// <summary>
        ///     Returns a success <see cref="Result" />.
        /// </summary>
        /// <returns>A new instance of <see cref="Result" />.</returns>
        public static Result Success() => new(true, error: null);

        /// <summary>
        ///     Returns a success <see cref="Result{TValue}" /> with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The result type.</typeparam>
        /// <param name="value">The result value.</param>
        /// <returns>A new instance of <see cref="Result{TValue}" /> with the specified value.</returns>
        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, null);

        public static Result SuccessWithMessage(string message) => new(true, message: message);
        public static Result<TValue> SuccessWithMessage<TValue>(TValue value, string message) => new(value, true, message);
    }
}
