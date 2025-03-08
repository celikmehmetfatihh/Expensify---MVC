using Expensify.Web.Models.Domain;

namespace Expensify.Web.Models.ViewModels
{
    public class AdminReportViewModel
    {
        public decimal TotalExpenses { get; set; }
        public int ApprovedFormsCount { get; set; }
        public int RejectedFormsCount { get; set; }
        public int PaidFormsCount { get; set; }
        public int CreatedFormsCount { get; set; }
        public int UpdatedFormsCount { get; set; }
        public List<ExpenseFormHistory> ExpenseFormHistories { get; set; }
    }
}
