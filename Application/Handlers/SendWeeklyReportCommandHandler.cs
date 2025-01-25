using Application.Commands;
using Application.Interfaces;
using Services.Interfaces;
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

            var emailBuilder = new AnalyticEmailBuilder($"..\\Services\\Helpers\\EmailTemplates\\AnalyticEmailTemplate.html");
            var emailBody = emailBuilder
                .SetDateRange(request.AnalyticEmailRequestDTO.DateRange.From, request.AnalyticEmailRequestDTO.DateRange.To)
                .SetIncomes(request.AnalyticEmailRequestDTO.Incomes)
                .SetExpenses(request.AnalyticEmailRequestDTO.Expenses)
                .SetBudgets(request.AnalyticEmailRequestDTO.Budgets)
                .SetTransactionsSummary(request.AnalyticEmailRequestDTO.TransactionsSummary)
                .Build();

            var result = await _emailService.SendEmailAsync(request.AnalyticEmailRequestDTO.ToEmail, request.AnalyticEmailRequestDTO.RecipientName, "Analytics Report", emailBody, cancellationToken);

            if (!result)
            {
                allEmailsSent = false;
            }

            return allEmailsSent;
        }
    }
}
