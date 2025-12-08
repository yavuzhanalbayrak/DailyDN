namespace DailyDN.Infrastructure.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(
            List<string> toList,
            string subject,
            string body,
            List<string>? ccList = null,
            List<string>? bccList = null);
    }
}