namespace Expensify.Web.Models.ViewModels
{
    public class ExpenseFormFilterViewModel
    {
        // Search filters
        public string? EmployeeName { get; set; }
        public string? CurrencyFilter { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? Status { get; set; }

        // The actual list of expense forms
        public IEnumerable<ExpenseFormViewModel> ExpenseForms { get; set; }
    }
}
