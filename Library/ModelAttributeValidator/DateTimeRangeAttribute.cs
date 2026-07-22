using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Library.ModelAttributeValidator
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class DateTimeRangeAttribute(string nameToShowInUI, bool required, string minimumDate, string maximumDate) : ValidationAttribute
    {
        private string NameToShowInUI { get; } = nameToShowInUI;
        private bool Required { get; } = required;
        private DateTime MinimumDate { get; } = DateTime.Parse(minimumDate);
        private DateTime MaximumDate { get; } = DateTime.Parse(maximumDate);
        
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string PropertyName = validationContext.MemberName ?? "Field";

            if (value == null)
            {
                return Required
                    ? new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} es requerido.")
                    : ValidationResult.Success;
            }

            if (value is not DateTime DateTimeToTest)
            {
                return new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} debe ser una fecha válida.");
            }

            if (DateTimeToTest < MinimumDate || DateTimeToTest > MaximumDate)
            {
                return new ValidationResult(
                    $"[{PropertyName}] El campo {NameToShowInUI} debe estar entre {MinimumDate:dd/MM/yyyy HH:mm:ss} y {MaximumDate:dd/MM/yyyy HH:mm:ss}.");
            }

            return ValidationResult.Success;
        }
    }
}