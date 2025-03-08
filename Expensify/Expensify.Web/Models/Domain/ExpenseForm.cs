using System.ComponentModel.DataAnnotations;

namespace Expensify.Web.Models.Domain
{
	public class ExpenseForm
	{
		public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SubmissionDate { get; set; }
		public bool IsApprovedByManager { get; set; } = false;
        public bool IsPaidByAccountant { get; set; } = false;
        public bool IsRejectedByManager { get; set; } = false;
        public string? RejectionReason { get; set; }

        public string EmployeeId { get; set; } // Foreign key to ApplicationUser (Employee)
        public ApplicationUser Employee { get; set; } // Navigation property to Employee

        public string? ManagerId { get; set; } // Foreign key to ApplicationUser (Manager)
        public ApplicationUser? Manager { get; set; } // Navigation property to Manager

        public string? AccountantId { get; set; } // Foreign key to ApplicationUser (Accountant)
        public ApplicationUser? Accountant { get; set; } // Navigation property to Accountant

		[Required]
		public string Currency { get; set; }

		[Required]
        public List<Expense> Expenses { get; set; } = new List<Expense>(); // Collection of expenses in the form
    }
}
