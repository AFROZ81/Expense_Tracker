using ExpenseTracker.Data;
using ExpenseTracker.Models.Entities;
using ExpenseTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            var vm = new ProfileViewModel
            {
                FullName = user.FullName ?? user.UserName ?? "User",
                Email = user.Email ?? string.Empty,
                CreatedOn = user.CreatedOn,
                AccountCount = _context.Accounts.Count(a => a.UserId == userId),
                CategoryCount = _context.Categories.Count(c => c.UserId == userId),
                ExpenseCount = _context.Expenses.Count(e => e.UserId == userId),
                IncomeCount = _context.Incomes.Count(i => i.UserId == userId),
                BudgetCount = _context.Budgets.Count(b => b.UserId == userId),
                TotalBalance = _context.Accounts.Where(a => a.UserId == userId).Sum(a => (decimal?)a.CurrentBalance) ?? 0,
                TotalIncome = _context.Incomes.Where(i => i.UserId == userId).Sum(i => (decimal?)i.Amount) ?? 0,
                TotalExpense = _context.Expenses.Where(e => e.UserId == userId).Sum(e => (decimal?)e.Amount) ?? 0,
                RecentAccounts = _context.Accounts
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.CreatedOn)
                    .Take(3)
                    .ToList()
            };

            return View(vm);
        }
    }
}
