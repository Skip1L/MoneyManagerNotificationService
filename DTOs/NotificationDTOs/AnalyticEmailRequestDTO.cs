using System.Windows.Input;

namespace DTOs.NotificationDTOs
{
    public class AnalyticEmailRequestDTO
    {
        public string RecipientName { get; set; }
        public string ToEmail { get; set; }
        public List<CategoryReportDTO> Incomes { get; set; }
        public List<CategoryReportDTO> Expenses { get; set; }
        public List<BudgetReportDTO> Budgets { get; set; }
        public TransactionsSummaryDTO TransactionsSummary { get; set; }
    }
}
