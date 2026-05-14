using System.Globalization;
using System.Security.Claims;
using ClosedXML.Excel;
using ExpenseTracker.Data;
using ExpenseTracker.Models.ViewModels;
using ExpenseTracker.Models.ViewModels.Exports;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ExportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IViewRenderService _viewRenderService;
        private readonly IExportService _exportService;

        public ExportController(
            ApplicationDbContext context,
            IViewRenderService viewRenderService,
            IExportService exportService)
        {
            _context = context;
            _viewRenderService = viewRenderService;
            _exportService = exportService;
        }

        [HttpGet]
        public async Task<IActionResult> ExpensesPdf()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Account)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();

            var model = new ExpenseReportExportViewModel
            {
                GeneratedFor = User.FindFirstValue("FullName") ?? User.Identity?.Name ?? "User",
                GeneratedOn = DateTime.Now,
                TotalRecords = expenses.Count,
                TotalAmount = expenses.Sum(x => x.Amount),
                Rows = expenses.Select(e => new ExpenseExportRowViewModel
                {
                    ExpenseDate = e.ExpenseDate,
                    CategoryName = e.Category?.Name ?? "General",
                    AccountName = e.Account?.Name ?? "-",
                    Description = string.IsNullOrWhiteSpace(e.Description) ? "No description" : e.Description!,
                    Amount = e.Amount
                }).ToList()
            };

            try
            {
                var html = await _viewRenderService.RenderToStringAsync(ControllerContext, "~/Views/ExportTemplates/ExpensesPdf.cshtml", model);
                var bytes = await _exportService.GeneratePdfAsync(html);
                var fileName = $"Transaction_History_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                return File(bytes, "application/pdf", fileName);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "PDF export service is currently unavailable.");
            }
        }

        [HttpGet]
        public IActionResult ExpensesExcel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var expenses = _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Account)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToList();

            var bytes = _exportService.GenerateExcel(workbook =>
            {
                var sheet = workbook.Worksheets.Add("Transactions");

                sheet.Cell(1, 1).Value = "Date";
                sheet.Cell(1, 2).Value = "Category";
                sheet.Cell(1, 3).Value = "Account";
                sheet.Cell(1, 4).Value = "Description";
                sheet.Cell(1, 5).Value = "Amount";

                var headerRange = sheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E8F0");

                var row = 2;
                foreach (var item in expenses)
                {
                    sheet.Cell(row, 1).Value = item.ExpenseDate;
                    sheet.Cell(row, 1).Style.DateFormat.Format = "dd-MMM-yyyy";
                    sheet.Cell(row, 2).Value = item.Category?.Name ?? "General";
                    sheet.Cell(row, 3).Value = item.Account?.Name ?? "-";
                    sheet.Cell(row, 4).Value = string.IsNullOrWhiteSpace(item.Description) ? "No description" : item.Description;
                    sheet.Cell(row, 5).Value = item.Amount;
                    sheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                    row++;
                }

                sheet.Cell(row, 4).Value = "Total";
                sheet.Cell(row, 4).Style.Font.Bold = true;
                sheet.Cell(row, 5).FormulaA1 = $"SUM(E2:E{row - 1})";
                sheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                sheet.Cell(row, 5).Style.Font.Bold = true;

                sheet.Columns().AdjustToContents();
                sheet.SheetView.FreezeRows(1);
            });

            var fileName = $"Transaction_History_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> MonthlySummaryPdf(int? month, int? year)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var selectedMonth = month ?? DateTime.Now.Month;
            var selectedYear = year ?? DateTime.Now.Year;

            var summary = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.ExpenseDate.Month == selectedMonth &&
                            e.ExpenseDate.Year == selectedYear &&
                            e.UserId == userId)
                .GroupBy(e => e.Category != null ? e.Category.Name : "General")
                .Select(g => new MonthlySummaryViewModel
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToListAsync();

            var total = summary.Sum(x => x.TotalAmount);
            var model = new MonthlySummaryReportExportViewModel
            {
                GeneratedFor = User.FindFirstValue("FullName") ?? User.Identity?.Name ?? "User",
                GeneratedOn = DateTime.Now,
                Month = selectedMonth,
                Year = selectedYear,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selectedMonth),
                TotalAmount = total,
                Rows = summary.Select(x => new MonthlySummaryExportRowViewModel
                {
                    CategoryName = x.CategoryName,
                    Amount = x.TotalAmount,
                    Percentage = total <= 0 ? 0 : Math.Round((x.TotalAmount / total) * 100, 2)
                }).ToList()
            };

            try
            {
                var html = await _viewRenderService.RenderToStringAsync(ControllerContext, "~/Views/ExportTemplates/MonthlySummaryPdf.cshtml", model);
                var bytes = await _exportService.GeneratePdfAsync(html);
                var fileName = $"Monthly_Expense_Statement_{selectedYear}_{selectedMonth:00}.pdf";
                return File(bytes, "application/pdf", fileName);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "PDF export service is currently unavailable.");
            }
        }

        [HttpGet]
        public IActionResult MonthlySummaryExcel(int? month, int? year)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var selectedMonth = month ?? DateTime.Now.Month;
            var selectedYear = year ?? DateTime.Now.Year;

            var summary = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.ExpenseDate.Month == selectedMonth &&
                            e.ExpenseDate.Year == selectedYear &&
                            e.UserId == userId)
                .GroupBy(e => e.Category != null ? e.Category.Name : "General")
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            var bytes = _exportService.GenerateExcel(workbook =>
            {
                var sheet = workbook.Worksheets.Add("Monthly Summary");
                sheet.Cell(1, 1).Value = "Month";
                sheet.Cell(1, 2).Value = "Year";
                sheet.Cell(1, 3).Value = "Category";
                sheet.Cell(1, 4).Value = "Amount";
                sheet.Cell(1, 5).Value = "Share %";

                var headerRange = sheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E8F0");

                var total = summary.Sum(x => x.Total);
                var row = 2;
                foreach (var item in summary)
                {
                    sheet.Cell(row, 1).Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selectedMonth);
                    sheet.Cell(row, 2).Value = selectedYear;
                    sheet.Cell(row, 3).Value = item.Category;
                    sheet.Cell(row, 4).Value = item.Total;
                    sheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                    sheet.Cell(row, 5).Value = total <= 0 ? 0 : item.Total / total;
                    sheet.Cell(row, 5).Style.NumberFormat.Format = "0.00%";
                    row++;
                }

                sheet.Cell(row, 3).Value = "Total";
                sheet.Cell(row, 3).Style.Font.Bold = true;
                sheet.Cell(row, 4).FormulaA1 = $"SUM(D2:D{row - 1})";
                sheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                sheet.Cell(row, 4).Style.Font.Bold = true;

                sheet.Columns().AdjustToContents();
                sheet.SheetView.FreezeRows(1);
            });

            var fileName = $"Monthly_Expense_Statement_{selectedYear}_{selectedMonth:00}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
