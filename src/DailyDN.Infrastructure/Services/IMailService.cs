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

        Task SendTemplateEmailAsync(
            List<string> toList,
            string subject,
            string templateName,
            Dictionary<string, string> templateParameters,
            List<string>? ccList = null,
            List<string>? bccList = null);
    }
}