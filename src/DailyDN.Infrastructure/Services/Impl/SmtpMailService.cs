using System.Net;
using System.Net.Mail;
using DailyDN.Infrastructure.Models;
using Microsoft.Extensions.Options;

namespace DailyDN.Infrastructure.Services.Impl
{
    public class SmtpMailService(IOptions<SmtpSettings> smtpOptions) : IMailService
    {
        private readonly SmtpSettings _smtpSettings = smtpOptions.Value;

        /// <summary>
        /// Mail gönderir. To, CC ve BCC listelerini destekler.
        /// </summary>
        public async Task SendEmailAsync(
            List<string> toList,
            string subject,
            string body,
            List<string>? ccList = null,
            List<string>? bccList = null)
        {
            if (toList == null || toList.Count == 0)
                throw new ArgumentException("At least one recipient must be specified.", nameof(toList));

            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.User),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            // To
            foreach (var to in toList)
                message.To.Add(to);

            // CC
            if (ccList != null)
            {
                foreach (var cc in ccList)
                    message.CC.Add(cc);
            }

            // BCC
            if (bccList != null)
            {
                foreach (var bcc in bccList)
                    message.Bcc.Add(bcc);
            }

            await client.SendMailAsync(message);
        }
    }
}