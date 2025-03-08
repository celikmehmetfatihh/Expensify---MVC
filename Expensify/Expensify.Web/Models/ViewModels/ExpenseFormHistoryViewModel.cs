using Expensify.Web.Models.Domain;

namespace Expensify.Web.Models.ViewModels
{
    public class ExpenseFormHistoryViewModel
    {
        // Search filters
        public string? EmployeeName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }

        // The actual list of expense form histories
        public IEnumerable<ExpenseFormHistory> Histories { get; set; } = new List<ExpenseFormHistory>();
    }
}

