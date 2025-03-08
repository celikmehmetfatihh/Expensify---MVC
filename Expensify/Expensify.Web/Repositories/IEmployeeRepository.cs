using Expensify.Web.Models.Domain;

namespace Expensify.Web.Repositories
{
    public interface IEmployeeRepository
    {
        Task<ExpenseForm> AddAsync(ExpenseForm expenseForm, Guid userId);
        Task<List<ExpenseForm>> GetAllByIdAsync(Guid id);
        Task RemoveExpenseAsync(Guid id);
        Task<ExpenseForm> GetByIdAsync(Guid id);
        Task<ExpenseForm?> UpdateAsync(ExpenseForm expenseForm, Guid userId);
    }
}
