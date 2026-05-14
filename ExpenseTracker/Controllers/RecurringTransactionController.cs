using ExpenseTracker.Data;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class RecurringTransactionController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public RecurringTransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var transactions = _context.RecurringTransactions
                .Include(r => r.Account)
                .Where(r => r.UserId == UserId)
                .ToList();
            return View(transactions);
        }

        public IActionResult Create()
        {
            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == UserId), "Id", "Name");
            // For simplicity, we'll use existing categories for now
            ViewBag.ExpenseCategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name");
            ViewBag.IncomeCategoryList = new SelectList(_context.IncomeCategories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RecurringTransaction transaction, int? ExpenseCategoryId, int? IncomeCategoryId)
        {
            if (ModelState.IsValid)
            {
                transaction.UserId = UserId;
                transaction.CategoryId = transaction.Type == TransactionType.Expense ? (ExpenseCategoryId ?? 0) : (IncomeCategoryId ?? 0);
                
                _context.RecurringTransactions.Add(transaction);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == UserId), "Id", "Name", transaction.AccountId);
            ViewBag.ExpenseCategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name");
            ViewBag.IncomeCategoryList = new SelectList(_context.IncomeCategories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name");
            return View(transaction);
        }

        public IActionResult Delete(int id)
        {
            var transaction = _context.RecurringTransactions.FirstOrDefault(r => r.Id == id && r.UserId == UserId);
            if (transaction != null)
            {
                _context.RecurringTransactions.Remove(transaction);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
