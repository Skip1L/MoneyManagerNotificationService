using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.NotificationDTOs;

namespace Services.TemplateGenerators
{
    public static class AnalyticEmailTemplateGenerator
    {
        public static string GenerateEmailBody(AnalyticEmailRequestDTO request)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"<p>Dear {request.RecipientName},</p>");
            sb.AppendLine("<p>Here is your weekly analytics report:</p>");

            // Income Table
            sb.AppendLine("<h3>Income Report</h3>");
            sb.AppendLine("<table border='1' style='border-collapse:collapse; width:100%;'>");
            sb.AppendLine("<thead><tr><th>Category</th><th>Amount</th><th>Percentage</th></tr></thead><tbody>");
            foreach (var income in request.Incomes)
            {
                sb.AppendLine($"<tr><td>{income.Name}</td><td>{income.Amount:C}</td><td>{income.Percentage}%</td></tr>");
            }
            sb.AppendLine("</tbody></table>");

            // Expense Table
            sb.AppendLine("<h3>Expense Report</h3>");
            sb.AppendLine("<table border='1' style='border-collapse:collapse; width:100%;'>");
            sb.AppendLine("<thead><tr><th>Category</th><th>Amount</th><th>Percentage</th></tr></thead><tbody>");
            foreach (var expense in request.Expenses)
            {
                sb.AppendLine($"<tr><td>{expense.Name}</td><td>{expense.Amount:C}</td><td>{expense.Percentage}%</td></tr>");
            }
            sb.AppendLine("</tbody></table>");

            // Budget Table
            sb.AppendLine("<h3>Budget Report</h3>");
            sb.AppendLine("<table border='1' style='border-collapse:collapse; width:100%;'>");
            sb.AppendLine("<thead><tr><th>Budget Name</th><th>Income</th><th>Expense</th></tr></thead><tbody>");
            foreach (var budget in request.Budgets)
            {
                sb.AppendLine($"<tr><td>{budget.Name}</td><td>{budget.Income:C}</td><td>{budget.Expense:C}</td></tr>");
            }
            sb.AppendLine("</tbody></table>");

            // Summary
            sb.AppendLine("<h3>Summary</h3>");
            sb.AppendLine($"<p>Total Income: {request.TransactionsSummary.TotalIncomes:C}</p>");
            sb.AppendLine($"<p>Total Expense: {request.TransactionsSummary.TotalExpenses:C}</p>");
            sb.AppendLine($"<p>Total: {request.TransactionsSummary.NetBalance:C}</p>");

            return sb.ToString();
        }
    }
}
