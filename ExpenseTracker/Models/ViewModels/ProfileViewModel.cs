using ExpenseTracker.Models.Entities;

namespace ExpenseTracker.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public int AccountCount { get; set; }
        public int CategoryCount { get; set; }
        public int ExpenseCount { get; set; }
        public int IncomeCount { get; set; }
        public int BudgetCount { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public IEnumerable<Account> RecentAccounts { get; set; } = Enumerable.Empty<Account>();
    }
}
