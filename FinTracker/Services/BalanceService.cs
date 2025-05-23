using FinTracker.Controllers;
using FinTracker.Models;
using FinTracker.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Services
{
    public class BalanceService
    {
        private readonly ILogger<AccountController> _logger;
        private readonly FintrackContext _context;

        public BalanceService(ILogger<AccountController> logger, FintrackContext context) 
        {
            _logger = logger;
            _context = context;
        }

        public async Task UpdateBalanceAsync()
        {
            decimal currentBalance = await _context.Transactions
                .SumAsync(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);

            var balanceEntity = await _context.Budget.FirstOrDefaultAsync();
            if (balanceEntity != null)
            {
                balanceEntity.BudgetAmount = currentBalance;
                _context.Budget.Update(balanceEntity);
            }
            else
            {
                var newBalance = new Budget
                {
                    BudgetAmount = currentBalance
                };
                _context.Budget.Add(newBalance);
            }

            var today = DateTime.Today;
            var existingSnapshot = await _context.BudgetSnapshots
                .FirstOrDefaultAsync(b => b.Date.Date == today);

            if (existingSnapshot != null)
            {
                existingSnapshot.Balance = currentBalance;
                _context.BudgetSnapshots.Update(existingSnapshot);
            }
            else
            {
                var snapshot = new BudgetSnapshots
                {
                    Balance = currentBalance,
                    Date = DateTime.Now
                };
                _context.BudgetSnapshots.Add(snapshot);
            }

            await _context.SaveChangesAsync();
        }
    }
}
