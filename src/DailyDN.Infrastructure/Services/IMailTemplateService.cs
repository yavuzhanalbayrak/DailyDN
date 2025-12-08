namespace DailyDN.Infrastructure.Services
{
    public interface IMailTemplateService
    {
        Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> placeholders);
    }
}