using ClosedXML.Excel;

namespace ExpenseTracker.Services
{
    public interface IExportService
    {
        Task<byte[]> GeneratePdfAsync(string html);
        byte[] GenerateExcel(Action<XLWorkbook> buildWorkbook);
    }
}
