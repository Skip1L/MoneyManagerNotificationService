using System.Text;
using DTOs.NotificationDTOs;

namespace Services.TemplateGenerators
{
    public class AnalyticEmailBuilder
    {
        private string _htmlTemplate;
        private string _dateFrom;
        private string _dateTo;
        private List<CategoryReportDTO> _incomes;
        private List<CategoryReportDTO> _expenses;
        private List<BudgetReportDTO> _budgets;
        private TransactionsSummaryDTO _transactionsSummary;

        public AnalyticEmailBuilder(string htmlTemplate)
        {
            _htmlTemplate = File.ReadAllText(htmlTemplate);
            _dateFrom = string.Empty;
            _dateTo = string.Empty;
            _incomes = new List<CategoryReportDTO>();
            _expenses = new List<CategoryReportDTO>();
            _budgets = new List<BudgetReportDTO>();
            _transactionsSummary = new TransactionsSummaryDTO();
        }

        public AnalyticEmailBuilder SetDateRange(DateTime dateFrom, DateTime dateTo)
        {
            _dateFrom = dateFrom.ToString("yyyy-MM-dd HH:mm");
            _dateTo = dateTo.ToString("yyyy-MM-dd HH:mm");
            return this;
        }

        public AnalyticEmailBuilder SetIncomes(List<CategoryReportDTO> incomes)
        {
            _incomes = incomes;
            return this;
        }

        public AnalyticEmailBuilder SetExpenses(List<CategoryReportDTO> expenses)
        {
            _expenses = expenses;
            return this;
        }

        public AnalyticEmailBuilder SetBudgets(List<BudgetReportDTO> budgets)
        {
            _budgets = budgets;
            return this;
        }

        public AnalyticEmailBuilder SetTransactionsSummary(TransactionsSummaryDTO summary)
        {
            _transactionsSummary = summary;
            return this;
        }

        public string Build()
        {
            _htmlTemplate = _htmlTemplate.Replace("{DateFrom}", _dateFrom)
                                         .Replace("{DateTo}", _dateTo)
                                         .Replace("{Incomes}", BuildCategoryReport(_incomes))
                                         .Replace("{Expenses}", BuildCategoryReport(_expenses))
                                         .Replace("{Budgets}", BuildBudgetReport(_budgets))
                                         .Replace("{TotalIncomes}", _transactionsSummary.TotalIncomes.ToString("C"))
                                         .Replace("{TotalExpenses}", _transactionsSummary.TotalExpenses.ToString("C"))
                                         .Replace("{NetBalance}", _transactionsSummary.NetBalance.ToString("C"));

            return _htmlTemplate;
        }

        private string BuildCategoryReport(List<CategoryReportDTO> categoryReports)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var category in categoryReports)
            {
                sb.AppendLine($"<tr><td>{category.Name}</td><td>{category.Amount:C}</td><td>{category.Percentage}%</td></tr>");
            }
            return sb.ToString();
        }

        private string BuildBudgetReport(List<BudgetReportDTO> budgetReports)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var budget in budgetReports)
            {
                sb.AppendLine($"<tr><td>{budget.Name}</td><td>{budget.Income:C}</td><td>{budget.Expense:C}</td></tr>");
            }
            return sb.ToString();
        }
    }
}
