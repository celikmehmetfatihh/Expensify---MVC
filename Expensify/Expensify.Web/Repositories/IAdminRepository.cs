using Expensify.Web.Models.Domain;
using Expensify.Web.Models.ViewModels;

namespace Expensify.Web.Repositories
{
    public interface IAdminRepository
    {
        Task<AdminReportViewModel> GetAdminReportAsync();
        Task<List<ExpenseFormHistory>> GetExpenseFormHistoriesAsync();
        Task<List<EmployeeExpenseSummary>> GetEmployeeExpenseSummaryAsync();

	}
}
