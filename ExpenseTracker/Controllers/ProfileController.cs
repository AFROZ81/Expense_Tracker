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
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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
                AutomationCount = _context.RecurringTransactions.Count(r => r.UserId == userId),
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

        // GET: Change Password
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();
            var model = new ChangePasswordViewModel();
            return View(model);
        }

        // POST: Change Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                FullName = user.FullName ?? user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Check email uniqueness if changed
            if (user.Email != model.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "Email is already in use by another account.");
                    return View(model);
                }

                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
                
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Email);
                if (!setUserNameResult.Succeeded)
                {
                    foreach (var error in setUserNameResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
            }

            // Update FullName
            if (user.FullName != model.FullName)
            {
                user.FullName = model.FullName;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
