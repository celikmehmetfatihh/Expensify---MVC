namespace Expensify.Web.Models.Domain
{
    public class ExpenseAction
    {
        public Guid Id { get; set; } // Primary Key
        public string Name { get; set; } // "Created", "Rejected", "Approved", "Paid", "Updated"
    }
}
