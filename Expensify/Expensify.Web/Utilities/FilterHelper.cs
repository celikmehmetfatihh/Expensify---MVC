using Expensify.Web.Models.Domain;
using Expensify.Web.Models.ViewModels;

namespace Expensify.Web.Utilities
{
    public class FilterHelper
    {
        public static List<ExpenseForm> ApplyFilters(List<ExpenseForm> expenseForms, ExpenseFormFilterViewModel filter, ILogger logger = null)
        {
            logger?.LogInformation("Applying filters to expense forms.");

            if (!string.IsNullOrEmpty(filter.EmployeeName))
            {
                expenseForms = expenseForms
                    .Where(x => x.Employee.FullName.Contains(filter.EmployeeName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                logger?.LogInformation($"Filtered by EmployeeName: {filter.EmployeeName}");
            }
            if (!string.IsNullOrEmpty(filter.CurrencyFilter))
            {
                expenseForms = expenseForms.Where(x => x.Currency.Equals(filter.CurrencyFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                logger?.LogInformation($"Filtered by Currency: {filter.CurrencyFilter}");
            }

            if (filter.FromDate.HasValue)
            {
                expenseForms = expenseForms.Where(x => x.SubmissionDate.Date >= filter.FromDate.Value.Date).ToList();
                logger?.LogInformation($"Filtered by FromDate: {filter.FromDate}");
            }

            if (filter.ToDate.HasValue)
            {
                expenseForms = expenseForms.Where(x => x.SubmissionDate.Date <= filter.ToDate.Value.Date).ToList();
                logger?.LogInformation($"Filtered by ToDate: {filter.ToDate}");
            }

            if (filter.MinAmount.HasValue)
            {
                expenseForms = expenseForms.Where(x => x.Expenses.Sum(e => e.Amount) >= filter.MinAmount.Value).ToList();
                logger?.LogInformation($"Filtered by MinAmount: {filter.MinAmount}");
            }

            if (filter.MaxAmount.HasValue)
            {
                expenseForms = expenseForms.Where(x => x.Expenses.Sum(e => e.Amount) <= filter.MaxAmount.Value).ToList();
                logger?.LogInformation($"Filtered by MaxAmount: {filter.MaxAmount}");
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                logger?.LogInformation($"Applying Status filter: {filter.Status}");

                switch (filter.Status)
                {
                    case "Approved":
                        expenseForms = expenseForms.Where(x => x.IsApprovedByManager).ToList();
                        break;
                    case "Paid":
                        expenseForms = expenseForms.Where(x => x.IsPaidByAccountant).ToList();
                        break;
                    case "Rejected":
                        expenseForms = expenseForms.Where(x => x.IsRejectedByManager).ToList();
                        break;
                    case "Pending":
                        expenseForms = expenseForms.Where(x => !x.IsApprovedByManager && !x.IsRejectedByManager && !x.IsPaidByAccountant).ToList();
                        break;
                }
            }

            return expenseForms;
        }
    }
}
