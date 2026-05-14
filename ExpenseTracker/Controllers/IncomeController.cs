using ExpenseTracker.Data;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class IncomeController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public IncomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Income
        public IActionResult Index()
        {
            var incomes = _context.Incomes
                .Include(i => i.IncomeCategory)
                .Where(i => i.UserId == UserId)
                .OrderByDescending(i => i.IncomeDate)
                .ToList();

            return View(incomes);
        }

        // GET: Income/Create
        public IActionResult Create()
        {
            ViewBag.IncomeCategoryList = new SelectList(_context.IncomeCategories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name");
            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == UserId), "Id", "Name");
            return View();
        }

        // POST: Income/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Income income)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    income.UserId = UserId;
                    _context.Incomes.Add(income);

                    // Update Account Balance
                    var account = _context.Accounts.FirstOrDefault(a => a.Id == income.AccountId && a.UserId == UserId);
                    if (account != null)
                    {
                        account.CurrentBalance += income.Amount;
                        _context.Accounts.Update(account);
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Error saving income record.");
                }
            }

            ViewBag.IncomeCategoryList = new SelectList(_context.IncomeCategories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name", income.IncomeCategoryId);
            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == UserId), "Id", "Name", income.AccountId);
            return View(income);
        }

        // GET: Income/Edit/5
        public IActionResult Edit(int id)
        {
            var income = _context.Incomes.FirstOrDefault(i => i.Id == id && i.UserId == UserId);
            if (income == null) return NotFound();

            ViewBag.IncomeCategoryList = new SelectList(_context.IncomeCategories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name", income.IncomeCategoryId);
            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == UserId), "Id", "Name", income.AccountId);
            return View(income);
        }

        // POST: Income/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Income income)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var oldIncome = _context.Incomes.AsNoTracking().FirstOrDefault(i => i.Id == income.Id && i.UserId == UserId);
                    if (oldIncome == null) return NotFound();

                    // Revert old balance
                    var oldAccount = _context.Accounts.FirstOrDefault(a => a.Id == oldIncome.AccountId && a.UserId == UserId);
                    if (oldAccount != null)
                    {
                        oldAccount.CurrentBalance -= oldIncome.Amount;
                        _context.Accounts.Update(oldAccount);
                    }

                    // Apply new balance
                    var newAccount = _context.Accounts.FirstOrDefault(a => a.Id == income.AccountId && a.UserId == UserId);
                    if (newAccount != null)
                    {
                        newAccount.CurrentBalance += income.Amount;
                        _context.Accounts.Update(newAccount);
                    }

                    income.UserId = UserId;
                    _context.Incomes.Update(income);
                    _context.SaveChanges();
                    transaction.Commit();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Error updating income record.");
                }
            }

            ViewBag.IncomeCategoryList = new SelectList(_context.IncomeCategories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name", income.IncomeCategoryId);
            ViewBag.AccountList = new SelectList(_context.Accounts.Where(a => a.IsActive && a.UserId == UserId), "Id", "Name", income.AccountId);
            return View(income);
        }

        // GET: Income/Delete/5
        public IActionResult Delete(int id)
        {
            var income = _context.Incomes.FirstOrDefault(i => i.Id == id && i.UserId == UserId);
            if (income == null) return NotFound();

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Revert account balance
                var account = _context.Accounts.FirstOrDefault(a => a.Id == income.AccountId && a.UserId == UserId);
                if (account != null)
                {
                    account.CurrentBalance -= income.Amount;
                    _context.Accounts.Update(account);
                }

                _context.Incomes.Remove(income);
                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
