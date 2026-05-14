namespace ExpenseTracker.Models.ViewModels.Exports
{
    public class MonthlySummaryExportRowViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
