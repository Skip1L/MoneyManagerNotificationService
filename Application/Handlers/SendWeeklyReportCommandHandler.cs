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

        public async Task<bool> Handle(SendWeeklyReportCommand request, CancellationToken cancellationToken)
        {
            bool allEmailsSent = true;

            {
                var emailBody = AnalyticEmailTemplateGenerator.GenerateEmailBody(request.AnalyticEmailRequestDTO);

                // Send the email
                var result = await _emailService.SendEmailAsync(request.AnalyticEmailRequestDTO.ToEmail, request.AnalyticEmailRequestDTO.RecipientName, "Weekly Analytics Report", emailBody, cancellationToken);

                if (!result)
                {
                    allEmailsSent = false;
                }
            }
            

            return allEmailsSent;
        }
    }
}
