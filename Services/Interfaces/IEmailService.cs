namespace Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string recipientName, string subject, string body, CancellationToken cancellationToken);
    }
}
