using System.Text.Json.Serialization;

namespace DailyDN.Application.Common.Model
{
    public class Result<TValue> : Result
    {
        [JsonConstructor]
        public Result()
        {

        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Result{TValueType}" /> class with the specified parameters.
        /// </summary>
        /// <param name="value">The result value.</param>
        /// <param name="isSuccess">The flag indicating if the result is successful.</param>
        /// <param name="error">The error.</param>
        protected internal Result(TValue? value, bool isSuccess, Error? error)
            : base(isSuccess, error)
        {
            Data = value;
        }


        protected internal Result(TValue? value, bool isSuccess, string message) : this(value, isSuccess, null)
        {
            Message = message;
        }

        /// <summary>
        ///     Gets the result value if the result is successful, otherwise throws an exception.
        /// </summary>
        /// <returns>The result value if the result is successful.</returns>
        /// <exception cref="InvalidOperationException"> when <see cref="Result.IsFailure" /> is true.</exception>
        public TValue? Data { get; set; }


        public static implicit operator Result<TValue>(TValue? value) => Create(value);
    }
}
