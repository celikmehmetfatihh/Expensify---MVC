using Expensify.Web.Models.ViewModels;

namespace Expensify.Web.Models.Domain
{
    public class ExpenseFormHistory
    {
        public Guid Id { get; set; }
        public Guid ExpenseFormId { get; set; }
        public string PerformedByUserId { get; set; }
        public Guid ActionId { get; set; }
        public DateTime Timestamp { get; set; }

        // Navigation Properties
        public ApplicationUser User { get; set; }
        public ExpenseForm ExpenseForm { get; set; }
        public ExpenseAction Action { get; set; }

    }
}
