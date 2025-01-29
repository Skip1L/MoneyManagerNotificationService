using System.Text;
using DTOs.NotificationDTOs;

namespace Services.TemplateGenerators
{
    public class AnalyticEmailBuilder
    {
        private readonly string _mainTemplatePath;
        private readonly string _headerTemplatePath;
        private readonly string _dateRangeTemplatePath;
        private readonly string _categoryTableTemplatePath;
        private readonly string _budgetTableTemplatePath;
        private readonly string _transactionSummaryTemplatePath;

        private readonly StringBuilder _stringBuilder;

        public AnalyticEmailBuilder()
        {
            _mainTemplatePath = $"..\\Services\\Helpers\\EmailTemplates\\AnalyticEmailTemplate.html";
            _headerTemplatePath = $"..\\Services\\Helpers\\EmailTemplates\\header-template.html";
            _dateRangeTemplatePath = $"..\\Services\\Helpers\\EmailTemplates\\date-range-template.html";
            _categoryTableTemplatePath = $"..\\Services\\Helpers\\EmailTemplates\\category-table-template.html";
            _budgetTableTemplatePath = $"..\\Services\\Helpers\\EmailTemplates\\budget-table-template.html";
            _transactionSummaryTemplatePath = $"..\\Services\\Helpers\\EmailTemplates\\transaction-summary-template.html";

            _stringBuilder = new StringBuilder();
        }

        public AnalyticEmailBuilder AddHeader()
        {
            string headerTemplate = File.ReadAllText(_headerTemplatePath);
            _stringBuilder.AppendLine(headerTemplate);
            return this;
        }

        public AnalyticEmailBuilder AddDateRange(string dateFrom, string dateTo)
        {
            string dateRangeTemplate = File.ReadAllText(_dateRangeTemplatePath);
            string populatedDateRange = dateRangeTemplate
                .Replace("{DateFrom}", dateFrom)
                .Replace("{DateTo}", dateTo);

            _stringBuilder.AppendLine(populatedDateRange);
            return this;
        }

        public AnalyticEmailBuilder AddCategoryTable(List<CategoryReportDTO> categoryReports, string tableType)
        {
            string categoryTableTemplate = File.ReadAllText(_categoryTableTemplatePath);
            string populatedTable = BuildCategoryTable(categoryTableTemplate, categoryReports, tableType);
            _stringBuilder.AppendLine(populatedTable);
            return this;
        }

        public AnalyticEmailBuilder AddBudgetTable(List<BudgetReportDTO> budgets)
        {
            string budgetTableTemplate = File.ReadAllText(_budgetTableTemplatePath);
            string populatedTable = BuildBudgetTable(budgetTableTemplate, budgets);
            _stringBuilder.AppendLine(populatedTable);
            return this;
        }

        public AnalyticEmailBuilder AddTransactionSummary(TransactionsSummaryDTO summary)
        {
            string summaryTemplate = File.ReadAllText(_transactionSummaryTemplatePath);
            string populatedSummary = summaryTemplate
                .Replace("{TotalIncomes}", summary.TotalIncomes.ToString("C"))
                .Replace("{TotalExpenses}", summary.TotalExpenses.ToString("C"))
                .Replace("{NetBalance}", summary.NetBalance.ToString("C"));

            _stringBuilder.AppendLine(populatedSummary);
            return this;
        }

        public string Build()
        {
            string mainTemplate = File.ReadAllText(_mainTemplatePath);
            string fullContent = mainTemplate.Replace("{Content}", _stringBuilder.ToString());
            return fullContent;
        }

        private string BuildCategoryTable(string template, List<CategoryReportDTO> categoryReports, string tableType)
        {
            string tableTitle = tableType.Equals("Incomes", StringComparison.OrdinalIgnoreCase) ? "Incomes" : "Expenses";
            var rows = new StringBuilder();
            foreach (var category in categoryReports)
            {
                rows.AppendLine($"<tr><td>{category.Name}</td><td>{category.Amount:C}</td><td>{category.Percentage}%</td></tr>");
            }
            string populatedTable = template.Replace("{TableTitle}", tableTitle)
                                           .Replace("{TableRows}", rows.ToString());
            return populatedTable;
        }


        private string BuildBudgetTable(string template, List<BudgetReportDTO> budgetReports)
        {
            var rows = new StringBuilder();
            foreach (var budget in budgetReports)
            {
                rows.AppendLine($"<tr><td>{budget.Name}</td><td>{budget.Income:C}</td><td>{budget.Expense:C}</td></tr>");
            }
            string populatedTable = template.Replace("{TableRows}", rows.ToString());
            return populatedTable;
        }
    }


}
