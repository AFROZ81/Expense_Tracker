using ExpenseTracker.Data;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class BudgetController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Budget
        public IActionResult Index()
        {
            var now = DateTime.Now;
            var budgets = _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.Year == now.Year && b.UserId == UserId) // Show current year budgets for user
                .OrderByDescending(b => b.Month)
                .ToList();

            // Calculate actual spending for each budget
            foreach (var budget in budgets)
            {
                var actualSpending = _context.Expenses
                    .Where(e => e.CategoryId == budget.CategoryId && 
                                e.ExpenseDate.Month == budget.Month && 
                                e.ExpenseDate.Year == budget.Year &&
                                e.UserId == UserId)
                    .Sum(e => (decimal?)e.Amount) ?? 0;
                
                ViewData[$"Actual_{budget.Id}"] = actualSpending;
            }

            return View(budgets);
        }

        // GET: Budget/Create
        public IActionResult Create()
        {
            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name");
            return View();
        }

        // POST: Budget/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Budget budget)
        {
            if (ModelState.IsValid)
            {
                // Check if budget already exists for this category/month/year for THIS user
                var existing = _context.Budgets.FirstOrDefault(b => 
                    b.CategoryId == budget.CategoryId && 
                    b.Month == budget.Month && 
                    b.Year == budget.Year &&
                    b.UserId == UserId);

                if (existing != null)
                {
                    ModelState.AddModelError("", "A budget already exists for this category and month.");
                    TempData["ErrorMessage"] = "A budget already exists for this category and month.";
                }
                else
                {
                    budget.UserId = UserId;
                    _context.Budgets.Add(budget);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Budget created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name", budget.CategoryId);
            return View(budget);
        }

        // GET: Budget/Edit/5
        public IActionResult Edit(int id)
        {
            var budget = _context.Budgets.FirstOrDefault(b => b.Id == id && b.UserId == UserId);
            if (budget == null) return NotFound();

            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name", budget.CategoryId);
            return View(budget);
        }

        // POST: Budget/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Budget budget)
        {
            if (ModelState.IsValid)
            {
                budget.UserId = UserId;
                _context.Budgets.Update(budget);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Budget updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.IsActive && c.UserId == UserId), "Id", "Name", budget.CategoryId);
            return View(budget);
        }

        // GET: Budget/Delete/5
        public IActionResult Delete(int id)
        {
            var budget = _context.Budgets.FirstOrDefault(b => b.Id == id && b.UserId == UserId);
            if (budget != null)
            {
                try
                {
                    _context.Budgets.Remove(budget);
                    _context.SaveChanges();
                    TempData["DeleteMessage"] = "Budget deleted successfully!";
                }
                catch
                {
                    TempData["ErrorMessage"] = "Failed to delete budget.";
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
