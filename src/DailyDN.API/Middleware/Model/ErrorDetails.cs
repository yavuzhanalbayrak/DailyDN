namespace DailyDN.API.Middleware.Model
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }


        public string Message { get; set; } = string.Empty;

        public override string ToString() => $"{StatusCode}: {Message}";
    }
}
