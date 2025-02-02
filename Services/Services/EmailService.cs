using System.Net;
using System.Net.Mail;
using Services.Interfaces;

namespace Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private const string _fromEmail = "baton3245@gmail.com";

        public EmailService(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

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
