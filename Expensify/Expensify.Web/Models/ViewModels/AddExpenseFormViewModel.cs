using Expensify.Web.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Expensify.Web.Models.ViewModels
{
    public class AddExpenseFormViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime SubmissionDate { get; set; }

        [Required]
        public string EmployeeId { get; set; }

		[Required]
		public string Currency { get; set; }

		public List<Expense> Expenses { get; set; } = new List<Expense>();

        public bool AddMoreExpenses { get; set; } // Track if the user clicked "Add More"

    }
}
