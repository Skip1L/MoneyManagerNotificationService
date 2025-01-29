using System.Text;
using DTOs.NotificationDTOs;

namespace Services.TemplateGenerators
{
    public class AnalyticEmailBuilder
    {
        private static readonly string _mainTemplate;
        private static readonly string _headerTemplate;
        private static readonly string _dateRangeTemplate;
        private static readonly string _categoryTableTemplate;
        private static readonly string _budgetTableTemplate;
        private static readonly string _transactionSummaryTemplate;

        private readonly StringBuilder _stringBuilder;

        public AnalyticEmailBuilder()
        {
            _stringBuilder = new StringBuilder();
        }

        static AnalyticEmailBuilder()
        {
            _mainTemplate = File.ReadAllText($"..\\Services\\Helpers\\EmailTemplates\\AnalyticEmailTemplate.html");
            _headerTemplate = File.ReadAllText($"..\\Services\\Helpers\\EmailTemplates\\header-template.html");
            _dateRangeTemplate = File.ReadAllText($"..\\Services\\Helpers\\EmailTemplates\\date-range-template.html");
            _categoryTableTemplate = File.ReadAllText($"..\\Services\\Helpers\\EmailTemplates\\category-table-template.html");
            _budgetTableTemplate = File.ReadAllText($"..\\Services\\Helpers\\EmailTemplates\\budget-table-template.html");
            _transactionSummaryTemplate = File.ReadAllText($"..\\Services\\Helpers\\EmailTemplates\\transaction-summary-template.html");
        }

        public AnalyticEmailBuilder AddHeader()
        {
            _stringBuilder.AppendLine(_headerTemplate);
            return this;
        }

        public AnalyticEmailBuilder AddDateRange(string dateFrom, string dateTo)
        {
            var populatedDateRange = _dateRangeTemplate
                .Replace("{DateFrom}", dateFrom)
                .Replace("{DateTo}", dateTo);

            _stringBuilder.AppendLine(populatedDateRange);
            return this;
        }

        public AnalyticEmailBuilder AddCategoryTable(List<CategoryReportDTO> categoryReports, string tableType)
        {
            var populatedTable = BuildCategoryTable(_categoryTableTemplate, categoryReports, tableType);
            _stringBuilder.AppendLine(populatedTable);
            return this;
        }

        public AnalyticEmailBuilder AddBudgetTable(List<BudgetReportDTO> budgets)
        {
            var populatedTable = BuildBudgetTable(_budgetTableTemplate, budgets);
            _stringBuilder.AppendLine(populatedTable);
            return this;
        }

        public AnalyticEmailBuilder AddTransactionSummary(TransactionsSummaryDTO summary)
        {
            var populatedSummary = _transactionSummaryTemplate
                .Replace("{TotalIncomes}", summary.TotalIncomes.ToString("C"))
                .Replace("{TotalExpenses}", summary.TotalExpenses.ToString("C"))
                .Replace("{NetBalance}", summary.NetBalance.ToString("C"));

            _stringBuilder.AppendLine(populatedSummary);
            return this;
        }

        public string Build()
        {
            return _mainTemplate.Replace("{Content}", _stringBuilder.ToString());
        }

        private static string BuildCategoryTable(string template, List<CategoryReportDTO> categoryReports, string tableType)
        {
            var tableTitle = tableType.Equals("Incomes", StringComparison.OrdinalIgnoreCase) ? "Incomes" : "Expenses";
            var rows = new StringBuilder();
            foreach (var category in categoryReports)
            {
                rows.AppendLine($"<tr><td>{category.Name}</td><td>{category.Amount:C}</td><td>{category.Percentage}%</td></tr>");
            }
            var populatedTable = template
                .Replace("{TableTitle}", tableTitle)
                .Replace("{TableRows}", rows.ToString());

            return populatedTable;
        }

        private static string BuildBudgetTable(string template, List<BudgetReportDTO> budgetReports)
        {
            var rows = new StringBuilder();
            foreach (var budget in budgetReports)
            {
                rows.AppendLine($"<tr><td>{budget.Name}</td><td>{budget.Income:C}</td><td>{budget.Expense:C}</td></tr>");
            }
            var populatedTable = template.Replace("{TableRows}", rows.ToString());
            return populatedTable;
        }
    }


}
