using Expensify.Web.Data;
using Expensify.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;


namespace Expensify.Web.Repositories
{
    public class ExpenseFormRepository : IExpenseFormRepository
    {
        private readonly ExpensifyDbContext _expensifyDbContext;

        public ExpenseFormRepository(ExpensifyDbContext expensifyDbContext)
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
                .OrderByDescending(e => e.SubmissionDate)
                .ToListAsync();

			return expenseForms;
		}

		public async Task<ExpenseForm> GetByIdAsync(Guid id)
        {
            var expenseForm =  await _expensifyDbContext.ExpenseForms
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

        public async Task<List<ExpenseForm>> GetPendingsByManagerIdAsync(Guid id)
        {
            return await _expensifyDbContext.ExpenseForms
                .Include(x => x.Expenses)
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Where(x => x.ManagerId == id.ToString() && !x.IsApprovedByManager && !x.IsRejectedByManager)
                .OrderByDescending(e => e.SubmissionDate)
                .ToListAsync();
        }

        public async Task<ExpenseForm?> ApproveByManagerAsync(Guid formId, Guid userId)
        {
            var expenseForm = 
                await _expensifyDbContext.ExpenseForms
                .Include(x => x.Expenses)
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .FirstOrDefaultAsync(e => e.Id == formId);

            if (expenseForm == null)
            {
                return null;
            }

            expenseForm.IsApprovedByManager = true;

            var history = new ExpenseFormHistory
            {
                Id = Guid.NewGuid(),
                ExpenseFormId = expenseForm.Id,
                PerformedByUserId = userId.ToString(),
                ActionId = Guid.Parse("DB5641FD-1D42-471B-A525-63CB1B264AE5"), // "Approved" action
                Timestamp = DateTime.UtcNow
            };

            _expensifyDbContext.ExpenseFormHistories.Add(history);

            await _expensifyDbContext.SaveChangesAsync();

            return expenseForm;
        }

        public async Task<ExpenseForm?> RejectByManagerAsync(Guid id, string rejectionReason, Guid userId)
        {
            var expenseForm =
                await _expensifyDbContext.ExpenseForms
                .Include(x => x.Expenses)
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expenseForm == null)
            {
                return null;
            }

            expenseForm.IsRejectedByManager = true;
            expenseForm.RejectionReason = rejectionReason;

            var history = new ExpenseFormHistory
            {
                Id = Guid.NewGuid(),
                ExpenseFormId = expenseForm.Id,
                PerformedByUserId = userId.ToString(),
                ActionId = Guid.Parse("0C8332DB-8304-4CFB-BBD5-3B568CD8142A"), // "Rejected" action
                Timestamp = DateTime.UtcNow
            };

            _expensifyDbContext.ExpenseFormHistories.Add(history);

            await _expensifyDbContext.SaveChangesAsync();

            return expenseForm;
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

        public async Task<List<ExpenseForm>> GetApprovedAsync()
        {
            return await _expensifyDbContext.ExpenseForms
                .Include(e => e.Expenses)
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Where(x => x.IsApprovedByManager && !x.IsPaidByAccountant && !x.IsRejectedByManager)
                .OrderByDescending(e => e.SubmissionDate)
                .ToListAsync();
        }

        public async Task PayByAccountantAsync(Guid expenseFormId)
        {
            var expenseForm = await _expensifyDbContext.ExpenseForms
                .Include(x => x.Expenses)
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .OrderByDescending(e => e.SubmissionDate)
                .FirstOrDefaultAsync(e => e.Id == expenseFormId);

            if (expenseForm != null)
            {
                expenseForm.IsPaidByAccountant = true;

                // Record the "Paid" action in the history
                var history = new ExpenseFormHistory
                {
                    Id = Guid.NewGuid(),
                    ExpenseFormId = expenseForm.Id,
                    // There is only one accountant
                    PerformedByUserId = "e8a0dc0e-6c32-4a87-93b5-b05439ef05d1",
                    ActionId = Guid.Parse("D1626FFB-9D52-427F-9DAA-ADC270788E57"), // "Paid" action
                    Timestamp = DateTime.UtcNow
                };

                _expensifyDbContext.ExpenseFormHistories.Add(history);
                await _expensifyDbContext.SaveChangesAsync();
            }
        }
    }
}
