using MediatR;
using Microsoft.Extensions.Logging;
using Persistence.Commands.EmailCommands;
using Services.Enums;
using Services.Interfaces;
using Services.TemplateGenerators;

namespace Persistence.Handlers
{
    public class SendWeeklyReportCommandHandler(IEmailService emailService, ILogger<SendWeeklyReportCommandHandler> logger) : IRequestHandler<SendWeeklyReportCommand, bool>
    {
        private readonly ILogger<SendWeeklyReportCommandHandler> _logger = logger;
        private readonly IEmailService _emailService = emailService;

        public async Task<bool> Handle(SendWeeklyReportCommand request, CancellationToken cancellationToken)
        {
            var emailBody = new AnalyticEmailBuilder()
                .AddHeader()
                .AddDateRange(request.DateRange.From.ToString(), request.DateRange.To.ToString())
                .AddCategoryTable(request.Incomes, CategoryType.Income)
                .AddCategoryTable(request.Expenses, CategoryType.Expense)
                .AddBudgetTable(request.Budgets)
                .AddTransactionSummary(request.TransactionsSummary)
                .Build();

            _logger.LogInformation("Email Body Generated Successfully. Trying to send email.");

            return await _emailService.SendEmailAsync(request.ToEmail, request.RecipientName, "Analytics Report", emailBody, cancellationToken);
        }
    }
}
