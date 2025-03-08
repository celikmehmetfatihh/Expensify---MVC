namespace Expensify.Web.Models.ViewModels
{
    public class PendingPaymentViewModel : ExpenseFormFilterViewModel
    {
        public List<ExpenseFormViewModel> ExpenseForms { get; set; } = new List<ExpenseFormViewModel>();
    }
}
