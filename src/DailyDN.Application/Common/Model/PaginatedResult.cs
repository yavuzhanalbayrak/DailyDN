namespace DailyDN.Application.Common.Model
{
    public class PaginatedResult<T> : Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedResult{T}" /> class with the specified parameters.
        /// </summary>
        /// <param name="data">The paginated data.</param>
        /// <param name="totalItems">The total number of data across all pages.</param>
        /// <param name="currentPage">The current page number.</param>
        /// <param name="pageSize">The number of data per page.</param>
        /// <param name="error">The error that occurred.</param>
        private PaginatedResult(
            T? data,
            int totalItems,
            int currentPage,
            int pageSize,
            Error? error) : base(error == null, error)
        {
            if (IsSuccess)
            {
                Data = data;
                TotalItems = totalItems;
                CurrentPage = currentPage;
                PageSize = pageSize;
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            }
            else
            {
                Data = default;
                TotalItems = 0;
                CurrentPage = 1;
                PageSize = pageSize;
                TotalPages = 0;
            }
        }

        /// <summary>
        /// Determines whether this instance has a previous page.
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// Gets a value indicating whether this instance has a next page.
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        /// <summary>
        /// Gets the paginated data.
        /// </summary>
        public T? Data { get; }

        /// <summary>
        /// Gets the total number of data across all pages.
        /// </summary>
        public int TotalItems { get; }

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// Gets the number of data per page.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Returns a success <see cref="PaginatedResult{T}" /> with the specified data and pagination info.
        /// </summary>
        /// <param name="items">The paginated data.</param>
        /// <param name="totalItems">The total number of data across all pages.</param>
        /// <param name="currentPage">The current page number.</param>
        /// <param name="pageSize">The number of data per page.</param>
        /// <returns>A new instance of <see cref="PaginatedResult{T}" /> with the specified data.</returns>
        public static PaginatedResult<T> Success(
           T items,
            int totalItems,
            int currentPage,
            int pageSize) => new(items, totalItems, currentPage, pageSize, null);

        /// <summary>
        /// Returns a failure <see cref="PaginatedResult{T}" /> with the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="pageSize">Optional. The default page size if known. Defaults to 10.</param>
        /// <returns>A new instance of <see cref="PaginatedResult{T}" /> with the specified error.</returns>
        public static PaginatedResult<T> Failure(Error error, int pageSize = 10) => new(default, 0, 1, pageSize, error);
    }
}
