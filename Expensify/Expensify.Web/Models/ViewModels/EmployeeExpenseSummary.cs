namespace Expensify.Web.Models.ViewModels
{
	public class EmployeeExpenseSummary
	{
		public string EmployeeId { get; set; }
		public string EmployeeName { get; set; }
		public decimal TotalExpenses { get; set; }
		public int ExpenseFormCount { get; set; }
	}
}
