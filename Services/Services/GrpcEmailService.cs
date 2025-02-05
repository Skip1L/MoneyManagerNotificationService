using AutoMapper;
using DTOs.NotificationDTOs;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Services.Enums;
using Services.Interfaces;
using Services.Protos;
using Services.TemplateGenerators;

namespace Services.Services
{
    public class GrpcEmailService(IEmailService emailService, ILogger<GrpcEmailService> logger, IMapper mapper) : GrpcEmail.GrpcEmailBase
    {
        private readonly IEmailService _emailService = emailService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GrpcEmailService> _logger = logger;

        public override async Task<SendEmailResponse> SendEmail(SendEmailRequest request, ServerCallContext context)
        {
            var emailBody = new AnalyticEmailBuilder()
               .AddHeader()
               .AddDateRange(
                    request.DateRange.From.ToDateTime().ToString(), 
                    request.DateRange.To.ToDateTime().ToString())
               .AddCategoryTable(_mapper.Map<List<CategoryReportDTO>>(request.Incomes), CategoryType.Income)
               .AddCategoryTable(_mapper.Map<List<CategoryReportDTO>>(request.Expenses), CategoryType.Expense)
               .AddBudgetTable(_mapper.Map<List<BudgetReportDTO>>(request.Budgets))
               .AddTransactionSummary(_mapper.Map<TransactionsSummaryDTO>(request.TransactionsSummary))
            .Build();

            _logger.LogInformation("Email Body Generated Successfully. Trying to send email.");

            return new SendEmailResponse
            {
                Success = await _emailService.SendEmailAsync(request.ToEmail, request.RecipientName, "Analytics Report", emailBody, context.CancellationToken)
            };
        }
    }
}
