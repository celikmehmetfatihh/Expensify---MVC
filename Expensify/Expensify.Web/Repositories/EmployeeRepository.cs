using Expensify.Web.Data;
using Expensify.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Expensify.Web.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ExpensifyDbContext _expensifyDbContext;

        public EmployeeRepository(ExpensifyDbContext expensifyDbContext)
        {
            _expensifyDbContext = expensifyDbContext;
        }

        public async Task<ExpenseForm> AddAsync(ExpenseForm expenseForm, Guid userId)
        {
            await _expensifyDbContext.ExpenseForms.AddAsync(expenseForm);

            var history = new ExpenseFormHistory
            {
                Id = Guid.NewGuid(),
                ExpenseFormId = expenseForm.Id,
                PerformedByUserId = userId.ToString(),
                ActionId = Guid.Parse("CABD35AB-C820-4BA7-9054-8AAF85213E8E"), // "Created" action
                Timestamp = DateTime.UtcNow
            };

            _expensifyDbContext.ExpenseFormHistories.Add(history);

            await _expensifyDbContext.SaveChangesAsync();

            return expenseForm;
        }

        public async Task<List<ExpenseForm>> GetAllByIdAsync(Guid id)
        {
            var expenseForms = await _expensifyDbContext.ExpenseForms
                .Include(e => e.Expenses)
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Where(e => e.EmployeeId == id.ToString())
                .ToListAsync();

            return expenseForms;
        }

        public async Task<ExpenseForm> GetByIdAsync(Guid id)
        {
            var expenseForm = await _expensifyDbContext.ExpenseForms
                .Include(e => e.Expenses)
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expenseForm == null)
            {
                return null;
            }
            return expenseForm;
        }

        public async Task<ExpenseForm?> UpdateAsync(ExpenseForm expenseForm, Guid userId)
        {
            var existingForm = await _expensifyDbContext.ExpenseForms
                .Include(e => e.Expenses)
                .FirstOrDefaultAsync(e => e.Id == expenseForm.Id);

            if (existingForm != null)
            {
                existingForm.Currency = expenseForm.Currency;
                existingForm.IsApprovedByManager = expenseForm.IsApprovedByManager;
                existingForm.IsRejectedByManager = expenseForm.IsRejectedByManager;
                existingForm.IsPaidByAccountant = expenseForm.IsPaidByAccountant;
                existingForm.RejectionReason = expenseForm.RejectionReason;

                // Update or add expenses
                foreach (var expense in expenseForm.Expenses)
                {
                    var existingExpense = existingForm.Expenses.FirstOrDefault(e => e.Id == expense.Id);
                    if (existingExpense != null)
                    {
                        // Update existing expense
                        existingExpense.Description = expense.Description;
                        existingExpense.Amount = expense.Amount;
                        existingExpense.Date = expense.Date;
                    }
                    else
                    {
                        // Add new expense
                        existingForm.Expenses.Add(expense);
                    }
                }

                var history = new ExpenseFormHistory
                {
                    Id = Guid.NewGuid(),
                    ExpenseFormId = expenseForm.Id,
                    PerformedByUserId = userId.ToString(),
                    ActionId = Guid.Parse("6C00BAA4-8B4F-4170-B58F-800464817A1E"), // "Updated" action
                    Timestamp = DateTime.UtcNow
                };

                _expensifyDbContext.ExpenseFormHistories.Add(history);

                await _expensifyDbContext.SaveChangesAsync();

                return existingForm;
            }

            return null;
        }

        public async Task RemoveExpenseAsync(Guid id)
        {
            var expense = await _expensifyDbContext.Expenses.FindAsync(id);
            if (expense != null)
            {
                _expensifyDbContext.Expenses.Remove(expense);
                await _expensifyDbContext.SaveChangesAsync();
            }
        }

    }
}
