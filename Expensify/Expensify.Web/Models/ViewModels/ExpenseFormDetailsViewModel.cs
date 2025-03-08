using Expensify.Web.Models.Domain;

namespace Expensify.Web.Models.ViewModels
{
    public class ExpenseFormDetailsViewModel
    {
        public Guid Id { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string EmployeeFullName { get; set; }
        public string ManagerFullName { get; set; }
        public bool IsApprovedByManager { get; set; }
        public bool IsPaidByAccountant { get; set; }
        public string Currency { get; set; }
        public List<Expense> Expenses { get; set; }
    }
}
