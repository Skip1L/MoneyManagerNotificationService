using Application.Commands;
using Application.Common;
using Application.Interfaces;
using DTOs.NotificationDTOs;
using Services.Interfaces;
using Services.Services;
using Services.TemplateGenerators;

namespace Application.Handlers
{
    public class SendWeeklyReportCommandHandler : ICommandHandler<SendWeeklyReportCommand, bool>
    {
        private readonly IEmailService _emailService;

        public SendWeeklyReportCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<bool> Handle(SendWeeklyReportCommand requests, CancellationToken cancellationToken)
        {
            bool allEmailsSent = true;
            // Generate email body using the template generator

            foreach (var request in requests.AnalyticEmailRequestDTO)
            {
                var emailBody = AnalyticEmailTemplateGenerator.GenerateEmailBody(request);

                // Send the email
                var result = await _emailService.SendEmailAsync(request.ToEmail, request.RecipientName, "Weekly Analytics Report", emailBody, cancellationToken);

                if (!result)
                {
                    allEmailsSent = false;
                }
            }
            

            return allEmailsSent;
        }
    }
}
