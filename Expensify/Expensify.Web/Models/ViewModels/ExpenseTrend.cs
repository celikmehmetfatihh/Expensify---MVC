namespace Expensify.Web.Models.ViewModels
{
    public class ExpenseTrend
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal ApprovedExpenses { get; set; }
        public decimal RejectedExpenses { get; set; }
    }
}
