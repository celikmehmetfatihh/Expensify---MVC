using Expensify.Web.Models.ViewModels;
using Expensify.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expensify.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminRepository adminRepository, ILogger<AdminController> logger)
        {
            _adminRepository = adminRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ExpenseFormHistory(ExpenseFormHistoryViewModel filter)
        {
            _logger.LogInformation("Fetching expense form history.");

            // Retrieve all expense form histories from the database
            var histories = await _adminRepository.GetExpenseFormHistoriesAsync();

            _logger.LogInformation("Applying filters for expense form history.");

            // Apply filters in memory (after data is retrieved from the database)
            if (!string.IsNullOrEmpty(filter.EmployeeName))
            {
                histories = histories
                    .Where(h => h.User != null && h.User.FullName.Contains(filter.EmployeeName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                _logger.LogInformation($"Filtered by EmployeeName: {filter.EmployeeName}");
            }

            if (filter.FromDate.HasValue)
            {
                histories = histories
                    .Where(h => h.Timestamp.Date >= filter.FromDate.Value.Date)
                    .ToList();
                _logger.LogInformation($"Filtered by FromDate: {filter.FromDate}");
            }

            if (filter.ToDate.HasValue)
            {
                histories = histories
                    .Where(h => h.Timestamp.Date <= filter.ToDate.Value.Date)
                    .ToList();
                _logger.LogInformation($"Filtered by ToDate: {filter.ToDate}");
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                histories = histories
                    .Where(h => h.Action.Name.Equals(filter.Status, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                _logger.LogInformation($"Filtered by Status: {filter.Status}");
            }

            // Create the ViewModel and pass it to the view
            var viewModel = new ExpenseFormHistoryViewModel
            {
                EmployeeName = filter.EmployeeName,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                Status = filter.Status,
                Histories = histories
            };

            _logger.LogInformation("Returning view for expense form history.");

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> ExpenseTypeSummary()
        {
            _logger.LogInformation("Fetching expense type summary.");

            try
            {
                var report = await _adminRepository.GetAdminReportAsync();
                _logger.LogInformation("Returning expense type summary view.");
                return View(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the expense type summary.");
                TempData["ErrorMessage"] = "An error occurred while fetching the expense type summary. Please try again later.";
                return RedirectToAction("Error", "Home");
            }
        }

		[HttpGet]
		public async Task<IActionResult> EmployeeExpenseSummary()
		{
            _logger.LogInformation("Fetching employee expense summary.");

            try
            {
                var summary = await _adminRepository.GetEmployeeExpenseSummaryAsync();
                _logger.LogInformation("Returning employee expense summary view.");
                return View(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the employee expense summary.");
                TempData["ErrorMessage"] = "An error occurred while fetching the employee expense summary. Please try again later.";
                return RedirectToAction("Error", "Home");
            }
        }
	}
}
