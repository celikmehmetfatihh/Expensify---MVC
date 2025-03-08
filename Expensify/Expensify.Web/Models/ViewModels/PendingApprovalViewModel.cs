using System.ComponentModel.DataAnnotations;
using Expensify.Web.Models.Domain;

namespace Expensify.Web.Models.ViewModels
{
    public class PendingApprovalViewModel : ExpenseFormFilterViewModel
    {
        public string ManagerName { get; set; }

        public List<ExpenseForm> PendingExpenseForms { get; set; }

        [Required(ErrorMessage = "Please provide a reason for rejection.")]
        public string RejectionReason { get; set; }
    }
}
