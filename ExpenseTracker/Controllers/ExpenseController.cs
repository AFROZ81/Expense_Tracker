using ExpenseTracker.Data;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ExpenseTracker.Services;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ExpenseController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IRecurringTransactionService _recurringService;

        public ExpenseController(ApplicationDbContext context, IRecurringTransactionService recurringService)
        {
            _context = context;
            _recurringService = recurringService;
        }

        private ExpenseSetupViewModel GetExpensePrerequisites(string userId)
        {
            var hasAccounts = _context.Accounts.Any(a => a.IsActive && a.UserId == userId);
            var hasCategories = _context.Categories.Any(c => c.IsActive && c.UserId == userId);
            return new ExpenseSetupViewModel
            {
                HasAccounts = hasAccounts,
                HasCategories = hasCategories
            };
        }

        // GET: Expense
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var expenses = _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Account)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToList();

            return View(expenses);
        }

        // GET: Expense/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var prerequisites = GetExpensePrerequisites(userId);
            if (!prerequisites.HasAccounts || !prerequisites.HasCategories)
            {
                return View("SetupRequired", prerequisites);
            }

            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == userId), "Id", "Name");
            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == userId), "Id", "Name");
            return View();
        }

        // POST: Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense expense)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var prerequisites = GetExpensePrerequisites(userId);
            if (!prerequisites.HasAccounts || !prerequisites.HasCategories)
            {
                TempData["ErrorMessage"] = "Please add both an account and an expense category before recording expenses.";
                return View("SetupRequired", prerequisites);
            }

            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    expense.UserId = userId;
                    _context.Expenses.Add(expense);
                    
                    // Update Account Balance
                    var account = _context.Accounts.Find(expense.AccountId);
                    if (account != null)
                    {
                        account.CurrentBalance -= expense.Amount;
                        _context.Accounts.Update(account);
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    TempData["SuccessMessage"] = "Expense added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Failed to add expense. Please try again.";
                    ModelState.AddModelError("", "Error saving expense. Please try again.");
                }
            }

            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == userId), "Id", "Name", expense.CategoryId);
            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == userId), "Id", "Name", expense.AccountId);
            return View(expense);
        }

        // GET: Expense/Edit/5
        public IActionResult Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var expense = _context.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);
            if (expense == null) return NotFound();

            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == userId), "Id", "Name", expense.CategoryId);
            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == userId), "Id", "Name", expense.AccountId);
            return View(expense);
        }

        // POST: Expense/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Expense expense)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var oldExpense = _context.Expenses.AsNoTracking().FirstOrDefault(e => e.Id == expense.Id && e.UserId == UserId);
                    if (oldExpense == null) return NotFound();

                    // Revert old balance
                    var oldAccount = _context.Accounts.FirstOrDefault(a => a.Id == oldExpense.AccountId && a.UserId == UserId);
                    if (oldAccount != null)
                    {
                        oldAccount.CurrentBalance += oldExpense.Amount;
                        _context.Accounts.Update(oldAccount);
                    }

                    // Apply new balance
                    var newAccount = _context.Accounts.FirstOrDefault(a => a.Id == expense.AccountId && a.UserId == UserId);
                    if (newAccount != null)
                    {
                        newAccount.CurrentBalance -= expense.Amount;
                        _context.Accounts.Update(newAccount);
                    }

                    expense.UserId = UserId;
                    _context.Expenses.Update(expense);
                    _context.SaveChanges();
                    transaction.Commit();
                    TempData["SuccessMessage"] = "Expense updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Failed to update expense.";
                    ModelState.AddModelError("", "Error updating expense.");
                }
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == userId), "Id", "Name", expense.CategoryId);
            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == userId), "Id", "Name", expense.AccountId);
            return View(expense);
        }

        // GET: Expense/Delete/5
        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var expense = _context.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);
            if (expense == null) return NotFound();

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Revert account balance
                var account = _context.Accounts.FirstOrDefault(a => a.Id == expense.AccountId && a.UserId == userId);
                if (account != null)
                {
                    account.CurrentBalance += expense.Amount;
                    _context.Accounts.Update(account);
                }

                _context.Expenses.Remove(expense);
                _context.SaveChanges();
                transaction.Commit();
                TempData["DeleteMessage"] = "Expense deleted successfully!";
            }
            catch
            {
                transaction.Rollback();
                TempData["ErrorMessage"] = "Failed to delete expense.";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult MonthlySummary(int? month, int? year)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            var summary = _context.Expenses
                .Include(e => e.Category)
                .Where(e =>
                    e.ExpenseDate.Month == selectedMonth &&
                    e.ExpenseDate.Year == selectedYear &&
                    e.UserId == userId)
                .GroupBy(e => e.Category.Name)
                .Select(g => new MonthlySummaryViewModel
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .ToList();

            ViewBag.Month = selectedMonth;
            ViewBag.Year = selectedYear;

            return View(summary);
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null) await _recurringService.ProcessPendingTransactions(userId);

            var now = DateTime.Now;

            // Expenses logic
            var thisMonthExpense = _context.Expenses
                .Where(e => e.ExpenseDate.Month == now.Month &&
                            e.ExpenseDate.Year == now.Year &&
                            e.UserId == userId)
                .Sum(e => (decimal?)e.Amount) ?? 0;

            var lastMonthDate = now.AddMonths(-1);

            var lastMonthExpense = _context.Expenses
                .Where(e => e.ExpenseDate.Month == lastMonthDate.Month &&
                            e.ExpenseDate.Year == lastMonthDate.Year &&
                            e.UserId == userId)
                .Sum(e => (decimal?)e.Amount) ?? 0;

            var totalExpense = _context.Expenses
                .Where(e => e.UserId == userId)
                .Sum(e => (decimal?)e.Amount) ?? 0;

            // Income logic
            var totalIncome = _context.Incomes
                .Where(i => i.UserId == userId)
                .Sum(i => (decimal?)i.Amount) ?? 0;

            var thisMonthIncome = _context.Incomes
                .Where(i => i.IncomeDate.Month == now.Month &&
                            i.IncomeDate.Year == now.Year &&
                            i.UserId == userId)
                .Sum(i => (decimal?)i.Amount) ?? 0;

            var categorySummary = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category.Name)
                .Select(g => new MonthlySummaryViewModel
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .ToList();

            // Budget logic for current month
            var budgetSummary = _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.Month == now.Month && b.Year == now.Year && b.UserId == userId)
                .ToList()
                .Select(b => new BudgetStatusViewModel
                {
                    CategoryName = b.Category?.Name ?? "Unknown",
                    BudgetAmount = b.Amount,
                    ActualSpent = _context.Expenses
                        .Where(e => e.CategoryId == b.CategoryId && 
                                    e.ExpenseDate.Month == now.Month && 
                                    e.ExpenseDate.Year == now.Year &&
                                    e.UserId == userId)
                        .Sum(e => (decimal?)e.Amount) ?? 0
                })
                .ToList();

            var vm = new DashboardViewModel
            {
                TotalExpense = totalExpense,
                ThisMonthExpense = thisMonthExpense,
                LastMonthExpense = lastMonthExpense,
                TotalIncome = totalIncome,
                ThisMonthIncome = thisMonthIncome,
                CategorySummary = categorySummary,
                BudgetSummary = budgetSummary,
                Accounts = _context.Accounts.Where(a => a.IsActive && a.UserId == userId).ToList()
            };

            return View(vm);
        }

        public IActionResult Charts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // PIE CHART (Category-wise)
            var categoryData = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .ToList();

            var pieChart = new ChartDataViewModel
            {
                Labels = categoryData.Select(x => x.Category).ToList(),
                Values = categoryData.Select(x => x.Total).ToList()
            };

            // BAR CHART (Monthly)
            var monthlyData = _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            var barChart = new ChartDataViewModel
            {
                Labels = monthlyData
                    .Select(x => $"{x.Month}/{x.Year}")
                    .ToList(),
                Values = monthlyData
                    .Select(x => x.Total)
                    .ToList()
            };

            ViewBag.PieChart = pieChart;
            ViewBag.BarChart = barChart;

            return View();
        }
    }
}
