namespace ExpenseTracker.Models.ViewModels
{
    public class MonthlySummaryViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
