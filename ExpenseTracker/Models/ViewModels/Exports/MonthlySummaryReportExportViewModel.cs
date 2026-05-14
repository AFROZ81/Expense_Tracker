namespace ExpenseTracker.Models.ViewModels.Exports
{
    public class MonthlySummaryReportExportViewModel
    {
        public string ReportTitle { get; set; } = "Monthly Expense Summary";
        public string GeneratedFor { get; set; } = string.Empty;
        public DateTime GeneratedOn { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<MonthlySummaryExportRowViewModel> Rows { get; set; } = new();
    }
}
