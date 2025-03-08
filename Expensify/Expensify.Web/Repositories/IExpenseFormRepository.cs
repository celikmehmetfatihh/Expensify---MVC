using Expensify.Web.Models.Domain;

namespace Expensify.Web.Repositories
{
    public interface IExpenseFormRepository
    {
        Task<ExpenseForm> AddAsync(ExpenseForm expenseForm, Guid userId);
        Task<List<ExpenseForm>> GetAllByIdAsync(Guid id);
        Task RemoveExpenseAsync(Guid id);
        Task<ExpenseForm> GetByIdAsync(Guid id);
        Task<ExpenseForm?> UpdateAsync(ExpenseForm expenseForm, Guid userId);

        Task<List<ExpenseForm>> GetPendingsByManagerIdAsync(Guid id);
        Task<ExpenseForm?> ApproveByManagerAsync(Guid formId, Guid userId);
        Task<ExpenseForm?> RejectByManagerAsync(Guid id, string rejectionReason, Guid userId);

        Task<List<ExpenseForm>> GetApprovedAsync();

        Task PayByAccountantAsync(Guid expenseFormId);

    }
}
