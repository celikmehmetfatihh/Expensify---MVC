using System.ComponentModel.DataAnnotations;

namespace Expensify.Web.Models.Validation
{
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = (DateTime)value;
            if (date > DateTime.Now)
            {
                return new ValidationResult(ErrorMessage ?? "Date cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
