using ExpenseTracker.Data;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class WalletController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public WalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account
        public IActionResult Index()
        {
            var accounts = _context.Accounts.Where(a => a.UserId == UserId).ToList();
            return View(accounts);
        }

        // GET: Account/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Account account)
        {
            if (ModelState.IsValid)
            {
                account.UserId = UserId;
                account.CurrentBalance = account.InitialBalance;
                _context.Accounts.Add(account);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Account created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Account/Edit/5
        public IActionResult Edit(int id)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Id == id && a.UserId == UserId);
            if (account == null) return NotFound();
            return View(account);
        }

        // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Account account)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = _context.Accounts.AsNoTracking().FirstOrDefault(a => a.Id == account.Id && a.UserId == UserId);
                if (existingAccount == null) return NotFound();

                // Recalculate balance if initial balance changed
                decimal balanceDiff = account.InitialBalance - existingAccount.InitialBalance;
                account.CurrentBalance = existingAccount.CurrentBalance + balanceDiff;
                account.UserId = UserId;

                _context.Accounts.Update(account);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Account updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Account/Delete/5
        public IActionResult Delete(int id)
        {
            var account = _context.Accounts.Include(a => a.Expenses).Include(a => a.Incomes).FirstOrDefault(a => a.Id == id && a.UserId == UserId);
            if (account == null) return NotFound();

            if ((account.Expenses?.Any() ?? false) || (account.Incomes?.Any() ?? false))
            {
                TempData["ErrorMessage"] = "Cannot delete account with existing transactions.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Accounts.Remove(account);
                _context.SaveChanges();
                TempData["DeleteMessage"] = "Account deleted successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to delete account.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
