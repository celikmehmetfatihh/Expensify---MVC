using System.ComponentModel.DataAnnotations;
using Expensify.Web.Models.Validation;

namespace Expensify.Web.Models.Domain
{
	public class Expense
	{
		public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0, 5000, ErrorMessage = "Amount should be between 0 and 5000")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DateNotInFuture(ErrorMessage = "Date cannot be in the future")]
        public DateTime Date { get; set; }


		// Foreign Key and Navigation Properties
		public Guid ExpenseFormId { get; set; } // Foreign key to ExpenseForm
		public ExpenseForm ExpenseForm { get; set; } // Navigation property to ExpenseForm
	}
}
