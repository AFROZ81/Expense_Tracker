using ExpenseTracker.Data;
using ExpenseTracker.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class IncomeCategoryController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public IncomeCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IncomeCategory
        public IActionResult Index()
        {
            var categories = _context.IncomeCategories.Where(c => c.UserId == UserId).ToList();
            return View(categories);
        }

        // GET: IncomeCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IncomeCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IncomeCategory category)
        {
            if (ModelState.IsValid)
            {
                category.UserId = UserId;
                _context.IncomeCategories.Add(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Income category added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: IncomeCategory/Edit/5
        public IActionResult Edit(int id)
        {
            var category = _context.IncomeCategories.FirstOrDefault(c => c.Id == id && c.UserId == UserId);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: IncomeCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(IncomeCategory category)
        {
            if (ModelState.IsValid)
            {
                category.UserId = UserId;
                _context.IncomeCategories.Update(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Income category updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: IncomeCategory/Delete/5
        public IActionResult Delete(int id)
        {
            var category = _context.IncomeCategories.FirstOrDefault(c => c.Id == id && c.UserId == UserId);

            if (category == null)
            {
                return NotFound();
            }

            try
            {
                _context.IncomeCategories.Remove(category);
                _context.SaveChanges();
                TempData["DeleteMessage"] = "Income category deleted successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to delete income category.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
