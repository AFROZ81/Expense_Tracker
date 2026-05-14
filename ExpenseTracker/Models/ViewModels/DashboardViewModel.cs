using ExpenseTracker.Models.Entities;

namespace ExpenseTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalExpense { get; set; }
        public decimal ThisMonthExpense { get; set; }
        public decimal LastMonthExpense { get; set; }

        public decimal TotalIncome { get; set; }
        public decimal ThisMonthIncome { get; set; }
        public decimal NetBalance => TotalIncome - TotalExpense;

        public List<MonthlySummaryViewModel> CategorySummary { get; set; } = new();
        public List<BudgetStatusViewModel> BudgetSummary { get; set; } = new();
        public List<Account> Accounts { get; set; } = new();
    }
}
