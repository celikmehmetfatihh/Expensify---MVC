using Expensify.Web.Data;
using Expensify.Web.Models.Domain;
using Expensify.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Expensify.Web.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ExpensifyDbContext _expensifyDbContext;

        public AdminRepository(ExpensifyDbContext expensifyDbContext)
        {
            _expensifyDbContext = expensifyDbContext;
        }

        public async Task<AdminReportViewModel> GetAdminReportAsync()
        {
            var approvedActionId = Guid.Parse("DB5641FD-1D42-471B-A525-63CB1B264AE5");
            var rejectedActionId = Guid.Parse("0C8332DB-8304-4CFB-BBD5-3B568CD8142A");
            var paidActionId = Guid.Parse("D1626FFB-9D52-427F-9DAA-ADC270788E57");
            var createdActionId = Guid.Parse("CABD35AB-C820-4BA7-9054-8AAF85213E8E");
            var updatedActionId = Guid.Parse("6C00BAA4-8B4F-4170-B58F-800464817A1E");

            var expenseFormHistories = await _expensifyDbContext.ExpenseFormHistories
                .Include(h => h.ExpenseForm)
                    .ThenInclude(ef => ef.Expenses)
                .Include(h => h.Action)
                .Include(h => h.User)
                .ToListAsync();

            var approvedFormsCount = expenseFormHistories.Count(h => h.ActionId == approvedActionId);
            var rejectedFormsCount = expenseFormHistories.Count(h => h.ActionId == rejectedActionId);
            var paidFormsCount = expenseFormHistories.Count(h => h.ActionId == paidActionId);
            var createdFormsCount = expenseFormHistories.Count(h => h.ActionId == createdActionId);
            var updatedFormsCount = expenseFormHistories.Count(h => h.ActionId == updatedActionId);
            var totalExpenses = expenseFormHistories
                .Where(h => h.ActionId == paidActionId)
                .Sum(h => h.ExpenseForm.Expenses.Sum(e => e.Amount));

            var report = new AdminReportViewModel
            {
                TotalExpenses = totalExpenses,
                ApprovedFormsCount = approvedFormsCount,
                RejectedFormsCount = rejectedFormsCount,
                PaidFormsCount = paidFormsCount,
                CreatedFormsCount = createdFormsCount,
                UpdatedFormsCount = updatedFormsCount,
                ExpenseFormHistories = expenseFormHistories
            };

            return report;
        }

        public async Task<List<ExpenseFormHistory>> GetExpenseFormHistoriesAsync()
        {
            return await _expensifyDbContext.ExpenseFormHistories
                .Include(h => h.ExpenseForm)
                .Include(h => h.Action)
                .Include(h => h.User)
                .OrderByDescending(h => h.Timestamp) // Order by TimeStamp by descending order.
                .ToListAsync();
        }
		public async Task<List<EmployeeExpenseSummary>> GetEmployeeExpenseSummaryAsync()
		{
			var expenseForms = await _expensifyDbContext.ExpenseForms
				.Include(ef => ef.Expenses)
				.Include(ef => ef.Employee)
				.ToListAsync();

			var employeeExpenses = expenseForms
				.GroupBy(ef => new { ef.EmployeeId, ef.Employee.FullName })
				.Select(g => new EmployeeExpenseSummary
				{
					EmployeeId = g.Key.EmployeeId,
					EmployeeName = g.Key.FullName,
					TotalExpenses = g.Sum(x => x.Expenses.Sum(e => e.Amount)),
					ExpenseFormCount = g.Count()
				})
				.OrderByDescending(e => e.TotalExpenses)
				.ToList();

			return employeeExpenses;
		}

	}
}
