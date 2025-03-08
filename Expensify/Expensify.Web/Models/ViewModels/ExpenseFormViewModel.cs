using Expensify.Web.Models.Domain;

namespace Expensify.Web.Models.ViewModels
{
    public class ExpenseFormViewModel
    {
        public Guid Id { get; set; }
        public DateTime SubmissionDate { get; set; }
        public bool IsApprovedByManager { get; set; }
        public bool IsPaidByAccountant { get; set; }
        public bool IsRejectedByManager { get; set; }
        public string? RejectionReason { get; set; }
        public string Currency { get; set; }
        public List<Expense> Expenses { get; set; } = new List<Expense>();

        public ApplicationUser User { get; set; }

    }
}
