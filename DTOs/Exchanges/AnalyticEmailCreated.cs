using DTOs.CommonDTOs;
using DTOs.NotificationDTOs;

namespace DTOs.Exchanges
{
    public interface AnalyticEmailCreated
    {
        public List<BudgetReportDTO> Budgets { get; set; }
        public DateRangeFilter DateRange { get; set; }
        public List<CategoryReportDTO> Expenses { get; set; }
        public Guid Id { get; set; }
        public List<CategoryReportDTO> Incomes { get; set; }
        public string RecipientName { get; set; }
        public string ToEmail { get; set; }
        public TransactionsSummaryDTO TransactionsSummary { get; set; }
    }
}