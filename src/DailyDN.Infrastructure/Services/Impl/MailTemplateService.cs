using System.Reflection;
using System.Text;

namespace DailyDN.Infrastructure.Services.Impl
{
    public class MailTemplateService : IMailTemplateService
    {
        public async Task<string> GetTemplateAsync(
            string templateName,
            Dictionary<string, string> placeholders)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName =
                $"DailyDN.Infrastructure.Email.Templates.{templateName}";

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Embedded email template not found: {resourceName}");

            using var reader = new StreamReader(stream, Encoding.UTF8);

            var html = await reader.ReadToEndAsync();

            foreach (var placeholder in placeholders)
                html = html.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);

            return html;
        }
    }
}
