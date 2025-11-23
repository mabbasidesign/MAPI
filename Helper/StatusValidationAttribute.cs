using System.ComponentModel.DataAnnotations;

namespace MAPI.Helper
{
    public class StatusValidationAttribute : ValidationAttribute
    {
        private readonly string[] _statuses = new[] { "available", "assigned", "in maintenance" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var status = value as string;

            if (string.IsNullOrWhiteSpace(status))
            {
                return new ValidationResult("Status is required");
            }

            if (!_statuses.Contains(status.ToLower()))
            {
                return new ValidationResult(ErrorMessage ?? $"Status must be one of {string.Join(", ", _statuses)}");
            }

            return ValidationResult.Success;
        }
    }
}
