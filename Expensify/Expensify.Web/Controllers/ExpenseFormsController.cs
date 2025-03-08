using Expensify.Web.Models;
using Expensify.Web.Models.Domain;
using Expensify.Web.Models.ViewModels;
using Expensify.Web.Repositories;
using Expensify.Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Expensify.Web.Controllers
{
    [Authorize]
    public class ExpenseFormsController : Controller
    {
        private readonly IExpenseFormRepository _expenseFormRepository;
        private readonly ILogger<ExpenseFormsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExpenseFormsController(IExpenseFormRepository expenseFormRepository,
            ILogger<ExpenseFormsController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _expenseFormRepository = expenseFormRepository;
            _logger = logger;
            _userManager = userManager;
        }

        // Only employees should be able to add expenses
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            _logger.LogInformation("Entering Add expense form view");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Add: Current user could not be found.");
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("Creating new expense form for employee with ID: {UserId}", user.Id);


            var expenseFormModel = new AddExpenseFormViewModel
            {
                SubmissionDate = DateTime.Now,
                EmployeeId = user.Id,
                Expenses = new List<Expense>
                {
                    new Expense() // Add the first empty expense to the model
                }
            };

            return View(expenseFormModel);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> Add(AddExpenseFormViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Add Post: Current user could not be found.");
                ModelState.AddModelError(string.Empty, "Unable to determine the current user.");
                return View(model);
            }

            if (model.AddMoreExpenses)
            {
                model.Expenses.Add(new Expense()); // Add another empty expense
                _logger.LogInformation("User {UserId} is adding more expenses.", user.Id);
                return View(model); // Return the view to add another expense without saving
            }


            var expenseForm = new ExpenseForm
            {
                SubmissionDate = DateTime.Now,
                EmployeeId = user.Id,
                Employee = user,
                ManagerId = user.ManagerId,
                AccountantId = "e8a0dc0e-6c32-4a87-93b5-b05439ef05d1", // Since there is only one Accountant, automatically assign.
                Currency = model.Currency,
                Expenses = model.Expenses
            };

            foreach (var expense in expenseForm.Expenses)
            {
                expense.ExpenseForm = expenseForm;
            }

            _logger.LogInformation("Saving new expense form for employee {UserId}.", user.Id);

            await _expenseFormRepository.AddAsync(expenseForm, Guid.Parse(user.Id));

            TempData["SuccessMessage"] = "Expense form successfully added.";

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> List(ExpenseFormFilterViewModel filter)
        {
            _logger.LogInformation("Fetching the current user for expense form list.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Current user could not be found.");
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("Fetching all expense forms for the current user.");

            var expenseForms = await _expenseFormRepository.GetAllByIdAsync(Guid.Parse(user.Id));

            var filteredExpenseForms = FilterHelper.ApplyFilters(expenseForms, filter, _logger);

            _logger.LogInformation("Returning filtered expense forms.");

            var viewModel = new ExpenseFormFilterViewModel
            {
                CurrencyFilter = filter.CurrencyFilter,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                MinAmount = filter.MinAmount,
                MaxAmount = filter.MaxAmount,
                Status = filter.Status,
                ExpenseForms = filteredExpenseForms.Select(expenseForm => new ExpenseFormViewModel
                {
                    Id = expenseForm.Id,
                    SubmissionDate = expenseForm.SubmissionDate,
                    IsApprovedByManager = expenseForm.IsApprovedByManager,
                    IsRejectedByManager = expenseForm.IsRejectedByManager,
                    IsPaidByAccountant = expenseForm.IsPaidByAccountant,
                    RejectionReason = expenseForm.RejectionReason,
                    Currency = expenseForm.Currency,
                    Expenses = expenseForm.Expenses
                }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            _logger.LogInformation($"Fetching expense form with ID {id} for editing.");

            var expenseForm = await _expenseFormRepository.GetByIdAsync(id);
            if (expenseForm == null)
            {
                _logger.LogError($"Edit: Expense form with ID {id} not found.");
                return RedirectToAction("List");
            }

            // Map the existing expense form to a view model
            var expenseFormViewModel = new ExpenseFormViewModel
            {
                Id = expenseForm.Id,
                SubmissionDate = expenseForm.SubmissionDate,
                IsApprovedByManager = expenseForm.IsApprovedByManager,
                IsRejectedByManager = expenseForm.IsRejectedByManager,
                IsPaidByAccountant = expenseForm.IsPaidByAccountant,
                RejectionReason = expenseForm.RejectionReason,
                Currency = expenseForm.Currency,
                Expenses = expenseForm.Expenses
            };

            _logger.LogInformation($"Returning view for editing expense form with ID {id}.");

            return View(expenseFormViewModel);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> Edit(ExpenseFormViewModel model)
        {
            _logger.LogInformation($"Editing expense form with ID {model.Id}.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Current user could not be found.");
                return RedirectToAction("Login", "Account");
            }

            var expenseForm = await _expenseFormRepository.GetByIdAsync(model.Id);
            if (expenseForm == null)
            {
                _logger.LogError($"Edit: Expense form with ID {model.Id} not found.");
                return RedirectToAction("List");
            }

            _logger.LogInformation($"Updating the currency and expenses for expense form with ID {model.Id}.");

            expenseForm.Currency = model.Currency;

            if (expenseForm.IsRejectedByManager)
            {
                _logger.LogInformation($"Resetting rejection status for expense form with ID {model.Id}.");
                // Reset the rejection status and reason
                expenseForm.IsRejectedByManager = false;
                expenseForm.RejectionReason = null;
                expenseForm.IsApprovedByManager = false;
            }

            // Update the Expenses list
            foreach (var expense in model.Expenses)
            {
                var existingExpense = expenseForm.Expenses.FirstOrDefault(e => e.Id == expense.Id);
                if (existingExpense != null)
                {
                    // Update existing expense
                    existingExpense.Description = expense.Description;
                    existingExpense.Amount = expense.Amount;
                    existingExpense.Date = expense.Date;
                }
                else
                {
                    // Add new expense
                    expense.ExpenseFormId = expenseForm.Id;
                    expenseForm.Expenses.Add(expense);
                }
            }

            //Remove deleted expenses

            var existingExpenseIds = expenseForm.Expenses.Select(e => e.Id).ToList();

            //  EXCEPT the list of ids representing the expenses that the user submitted in the form
            var expensesToRemove = existingExpenseIds.Except(model.Expenses.Select(e => e.Id)).ToList();
            foreach (var expenseId in expensesToRemove)
            {
                await _expenseFormRepository.RemoveExpenseAsync(expenseId);
            }

            // Update the repository with the changes
            await _expenseFormRepository.UpdateAsync(expenseForm, Guid.Parse(user.Id));

            TempData["SuccessMessage"] = "Expense form has been updated successfully.";
            _logger.LogInformation($"Expense form with ID {model.Id} updated successfully.");

            return RedirectToAction("List");
        }


        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> PendingApproval(ExpenseFormFilterViewModel filter)
        {
            _logger.LogInformation("Fetching current user for pending approval.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("PendingApproval: Current user could not be found.");
                TempData["ErrorMessage"] = "Unable to determine the current user.";
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation($"Fetching pending expense forms for Manager ID: {user.Id}");

            var pendingExpenseForms = await _expenseFormRepository.GetPendingsByManagerIdAsync(Guid.Parse(user.Id));

            var filteredExpenseForms = FilterHelper.ApplyFilters(pendingExpenseForms, filter, _logger);

            _logger.LogInformation("Returning view with filtered expense forms.");

            // Populate the view model
            var viewModel = new PendingApprovalViewModel
            {
                ManagerName = user.FullName,
                PendingExpenseForms = filteredExpenseForms.ToList(),
                EmployeeName = filter.EmployeeName,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                MinAmount = filter.MinAmount,
                MaxAmount = filter.MaxAmount
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> ApproveExpenseForm(Guid formId)
        {
            _logger.LogInformation($"Attempting to approve expense form with ID: {formId}");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("ApproveExpenseForm: Current user could not be found.");
                TempData["ErrorMessage"] = "Unable to determine the current user.";
                return RedirectToAction("Login", "Account");
            }

            if (formId == Guid.Empty)
            {
                _logger.LogError("Invalid expense form ID provided.");
                return RedirectToAction("PendingApproval", new { errorMessage = "Invalid expense form ID." });
            }

            try
            {
                var expenseForm = await _expenseFormRepository.ApproveByManagerAsync(formId, Guid.Parse(user.Id));

                if (expenseForm == null)
                {
                    _logger.LogWarning($"ApproveExpenseForm: Expense form with ID {formId} was not found.");
                    return RedirectToAction("PendingApproval", new { errorMessage = "The expense form could not be found. It may have been deleted or already approved." });
                }

                _logger.LogInformation($"ApproveExpenseForm: Expense form with ID {formId} has been successfully approved by the manager.");

                TempData["SuccessMessage"] = "Expense form has been approved successfully.";

                return RedirectToAction("PendingApproval");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while approving the expense form with ID {formId}.");
                return RedirectToAction("PendingApproval", new { errorMessage = "An error occurred while approving the expense form. Please try again later." });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> RejectExpenseForm(Guid formId, string rejectionReason)
        {
            _logger.LogInformation($"Attempting to reject expense form with ID: {formId}");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("RejectExpenseForm: Current user could not be found.");
                TempData["ErrorMessage"] = "Unable to determine the current user.";
                return RedirectToAction("Login", "Account");
            }

            if (formId == Guid.Empty || string.IsNullOrWhiteSpace(rejectionReason))
            {
                _logger.LogError("Invalid form ID or rejection reason provided.");
                TempData["ErrorMessage"] = "Invalid form ID or rejection reason. Please try again.";
                return RedirectToAction("PendingApproval");
            }

            try
            {
                var expenseForm = await _expenseFormRepository.RejectByManagerAsync(formId, rejectionReason, Guid.Parse(user.Id));

                if (expenseForm == null)
                {
                    _logger.LogWarning($"RejectExpenseForm: Expense form with ID {formId} was not found.");
                    TempData["ErrorMessage"] = "Expense form not found. Please try again.";
                    return RedirectToAction("PendingApproval");
                }

                _logger.LogInformation($"RejectExpenseForm: Expense form with ID {formId} has been successfully rejected by the manager.");

                TempData["SuccessMessage"] = "Expense form has been rejected successfully.";

                return RedirectToAction("PendingApproval");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RejectExpenseForm: An error occurred while rejecting the expense form with ID {formId}.");
                TempData["ErrorMessage"] = "An error occurred while rejecting the expense form. Please try again later.";
                return RedirectToAction("PendingApproval");
            }
        }



        [HttpGet]
        public async Task<IActionResult> ViewExpenseFormDetails(Guid id)
        {
            _logger.LogInformation($"Fetching details for expense form with ID: {id}");

            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid expense form ID provided.");
                return RedirectToAction("PendingApproval", new { errorMessage = "Invalid expense form ID." });
            }

            try
            {
                var expenseForm = await _expenseFormRepository.GetByIdAsync(id);

                if (expenseForm == null)
                {
                    _logger.LogWarning($"Expense form with ID {id} not found.");
                    return RedirectToAction("PendingApproval", new { errorMessage = $"Expense form with ID {id} not found." });
                }

                var viewModel = new ExpenseFormDetailsViewModel
                {
                    Id = expenseForm.Id,
                    SubmissionDate = expenseForm.SubmissionDate,
                    EmployeeFullName = expenseForm.Employee?.FullName ?? "Employee not available",
                    ManagerFullName = expenseForm.Manager?.FullName ?? "Manager not available",
                    IsApprovedByManager = expenseForm.IsApprovedByManager,
                    IsPaidByAccountant = expenseForm.IsPaidByAccountant,
                    Currency = expenseForm.Currency,
                    Expenses = expenseForm.Expenses
                };

                _logger.LogInformation($"Returning details view for expense form with ID: {id}");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving expense form with ID {id}.");
                return RedirectToAction("PendingApproval", new { errorMessage = "An error occurred while retrieving the expense form. Please try again later." });
            }
        }


        [Authorize(Roles = "Accountant")]
        [HttpGet]
        public async Task<IActionResult> PendingPayment(ExpenseFormFilterViewModel filter)
        {
            _logger.LogInformation("Fetching approved expense forms for payment.");

            var approvedExpenseForms = await _expenseFormRepository.GetApprovedAsync();

            var filteredExpenseForms = FilterHelper.ApplyFilters(approvedExpenseForms, filter, _logger);


            _logger.LogInformation("Returning view with filtered expense forms pending payment.");

            var viewModel = new PendingPaymentViewModel
            {
                CurrencyFilter = filter.CurrencyFilter,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                MinAmount = filter.MinAmount,
                MaxAmount = filter.MaxAmount,
                ExpenseForms = filteredExpenseForms.Select(expenseForm => new ExpenseFormViewModel
                {
                    Id = expenseForm.Id,
                    SubmissionDate = expenseForm.SubmissionDate,
                    IsApprovedByManager = expenseForm.IsApprovedByManager,
                    IsRejectedByManager = expenseForm.IsRejectedByManager,
                    IsPaidByAccountant = expenseForm.IsPaidByAccountant,
                    RejectionReason = expenseForm.RejectionReason,
                    Currency = expenseForm.Currency,
                    Expenses = expenseForm.Expenses
                }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Accountant")]
        [HttpPost]
        public async Task<IActionResult> MarkAsPaid(Guid id)
        {
            _logger.LogInformation($"Marking expense form with ID {id} as paid.");

            await _expenseFormRepository.PayByAccountantAsync(id);

            TempData["SuccessMessage"] = "Expense form has been marked as paid.";

            _logger.LogInformation($"Expense form with ID {id} has been successfully marked as paid.");

            return RedirectToAction("PendingPayment");
        }
    }
}
