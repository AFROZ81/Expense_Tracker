using ExpenseTracker.Data;
using ExpenseTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public interface IRecurringTransactionService
    {
        Task ProcessPendingTransactions(string userId);
    }

    public class RecurringTransactionService : IRecurringTransactionService
    {
        private readonly ApplicationDbContext _context;

        public RecurringTransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ProcessPendingTransactions(string userId)
        {
            var now = DateTime.Now;
            var recurring = await _context.RecurringTransactions
                .Where(r => r.IsActive && r.UserId == userId && (r.EndDate == null || r.EndDate > now))
                .ToListAsync();

            foreach (var r in recurring)
            {
                DateTime nextRun = GetNextRunDate(r);
                
                while (nextRun <= now)
                {
                    // Create transaction
                    if (r.Type == TransactionType.Expense)
                    {
                        var expense = new Expense
                        {
                            Description = "[Recurring] " + r.Description,
                            Amount = r.Amount,
                            ExpenseDate = nextRun,
                            CategoryId = r.CategoryId,
                            AccountId = r.AccountId,
                            UserId = r.UserId
                        };
                        _context.Expenses.Add(expense);

                        // Update account
                        var account = await _context.Accounts.FindAsync(r.AccountId);
                        if (account != null) account.CurrentBalance -= r.Amount;
                    }
                    else
                    {
                        var income = new Income
                        {
                            Description = "[Recurring] " + r.Description,
                            Amount = r.Amount,
                            IncomeDate = nextRun,
                            IncomeCategoryId = r.CategoryId,
                            AccountId = r.AccountId,
                            UserId = r.UserId
                        };
                        _context.Incomes.Add(income);

                        // Update account
                        var account = await _context.Accounts.FindAsync(r.AccountId);
                        if (account != null) account.CurrentBalance += r.Amount;
                    }

                    r.LastProcessedDate = nextRun;
                    nextRun = GetNextRunDate(r);
                }
            }

            await _context.SaveChangesAsync();
        }

        private DateTime GetNextRunDate(RecurringTransaction r)
        {
            DateTime baseDate = r.LastProcessedDate ?? r.StartDate;
            
            // If it has never been processed, the first run is the StartDate
            if (r.LastProcessedDate == null) return r.StartDate;

            return r.Frequency switch
            {
                Frequency.Daily => baseDate.AddDays(1),
                Frequency.Weekly => baseDate.AddDays(7),
                Frequency.Monthly => baseDate.AddMonths(1),
                Frequency.Yearly => baseDate.AddYears(1),
                _ => baseDate.AddMonths(1)
            };
        }
    }
}
