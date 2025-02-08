using DTOs.Exchanges;
using MassTransit;
using Microsoft.Extensions.Logging;
using Services.Enums;
using Services.Interfaces;
using Services.TemplateGenerators;

namespace Services.Services
{
    public class AnalyticEmailCreatedConsumer(IEmailService emailService, ILogger<AnalyticEmailCreatedConsumer> logger) : IConsumer<AnalyticEmailCreated>
    {
        private readonly ILogger<AnalyticEmailCreatedConsumer> _logger = logger;
        private readonly IEmailService _emailService = emailService;
        public async Task Consume(ConsumeContext<AnalyticEmailCreated> context)
        {
            _logger.LogInformation("Receive Message");

            var emailBody = new AnalyticEmailBuilder()
                .AddHeader()
                .AddDateRange(context.Message.DateRange.From.ToString(), context.Message.DateRange.To.ToString())
                .AddCategoryTable(context.Message.Incomes, CategoryType.Income)
                .AddCategoryTable(context.Message.Expenses, CategoryType.Expense)
                .AddBudgetTable(context.Message.Budgets)
                .AddTransactionSummary(context.Message.TransactionsSummary)
                .Build();

            _logger.LogInformation("Email Body Generated Successfully. Trying to send email.");

            await _emailService.SendEmailAsync(context.Message.ToEmail, context.Message.RecipientName, "Analytics Report", emailBody, context.CancellationToken);
        }
    }
}
