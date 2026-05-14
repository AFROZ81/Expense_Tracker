namespace ExpenseTracker.Models.ViewModels
{
    public class ChartDataViewModel
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Values { get; set; } = new();
    }
}
