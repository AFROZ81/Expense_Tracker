namespace ExpenseTracker.Models.ViewModels.Exports
{
    public class ExpenseReportExportViewModel
    {
        public string ReportTitle { get; set; } = "Expense Transactions";
        public string GeneratedFor { get; set; } = string.Empty;
        public DateTime GeneratedOn { get; set; }
        public int TotalRecords { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ExpenseExportRowViewModel> Rows { get; set; } = new();
    }
}
