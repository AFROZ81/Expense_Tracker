using ClosedXML.Excel;
using Microsoft.Playwright;

namespace ExpenseTracker.Services
{
    public class ExportService : IExportService
    {
        public async Task<byte[]> GeneratePdfAsync(string html)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync(new BrowserNewPageOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
            });

            await page.SetContentAsync(html, new PageSetContentOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            return await page.PdfAsync(new PagePdfOptions
            {
                PrintBackground = true,
                Format = "A4",
                Margin = new Margin
                {
                    Top = "18mm",
                    Bottom = "18mm",
                    Left = "12mm",
                    Right = "12mm"
                }
            });
        }

        public byte[] GenerateExcel(Action<XLWorkbook> buildWorkbook)
        {
            using var workbook = new XLWorkbook();
            buildWorkbook(workbook);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
