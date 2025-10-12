using System.Net.Mail;
using Services.Configs;
using Services.Interfaces;

namespace Services.Services
{
    public class EmailService(SmtpClient smtpClient, SmtpConfig smtpConfig) : IEmailService
    {
        private readonly SmtpClient _smtpClient = smtpClient;
        private readonly string _fromEmail = smtpConfig.From;

        public async Task<bool> SendEmailAsync(string toEmail, string recipientName, string subject, string body, CancellationToken cancellationToken)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(toEmail));

            await _smtpClient.SendMailAsync(mailMessage);
            return true;
        }
    }
}
