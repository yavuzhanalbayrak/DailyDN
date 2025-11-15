namespace DailyDN.Infrastructure.Services
{
    public interface ISmsProvider
    {
        Task SendAsync(string phoneNumber, string message);
    }
}