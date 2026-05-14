using ExpenseTracker.Data;
using ExpenseTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetFullReport(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;

            var expenses = _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Account)
                .Where(e => e.ExpenseDate >= start && e.ExpenseDate <= end && e.UserId == UserId)
                .OrderBy(e => e.ExpenseDate)
                .ToList();

            var incomes = _context.Incomes
                .Include(i => i.IncomeCategory)
                .Include(i => i.Account)
                .Where(i => i.IncomeDate >= start && i.IncomeDate <= end && i.UserId == UserId)
                .OrderBy(i => i.IncomeDate)
                .ToList();

            var model = new FullReportViewModel
            {
                StartDate = start,
                EndDate = end,
                Expenses = expenses,
                Incomes = incomes,
                TotalExpense = expenses.Sum(e => e.Amount),
                TotalIncome = incomes.Sum(i => i.Amount)
            };

            return View(model);
        }
    }
}
