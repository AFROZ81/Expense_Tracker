using ExpenseTracker.Models.Entities;

namespace ExpenseTracker.Models.ViewModels
{
    public class FullReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Expense> Expenses { get; set; } = new();
        public List<Income> Incomes { get; set; } = new();
        public decimal TotalExpense { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal NetSavings => TotalIncome - TotalExpense;
    }
}
