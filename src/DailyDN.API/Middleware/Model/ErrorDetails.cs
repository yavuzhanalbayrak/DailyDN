namespace DailyDN.API.Middleware.Model
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }


        public string Message { get; set; }

        public override string ToString() => $"{StatusCode}: {Message}";
    }
}
