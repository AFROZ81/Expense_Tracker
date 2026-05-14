namespace ExpenseTracker.Models.ViewModels
{
    public class BudgetStatusViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal BudgetAmount { get; set; }
        public decimal ActualSpent { get; set; }
        public decimal PercentageUsed => BudgetAmount > 0 ? (ActualSpent / BudgetAmount * 100) : 0;
        public bool IsOverBudget => ActualSpent > BudgetAmount;
    }
}
