using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Library.ModelAttributeValidator
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class DecimalRangeAttribute(string nameToShowInUI, bool required, double minimumDecimal, double maximumDecimal) : ValidationAttribute
    {
        public string NameToShowInUI { get; } = nameToShowInUI;
        public decimal MinimumDecimal { get; } = Convert.ToDecimal(minimumDecimal);
        public decimal MaximumDecimal { get; } = Convert.ToDecimal(maximumDecimal);
        public bool Required { get; } = required;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string PropertyName = validationContext.MemberName ?? "Field";

            if (value == null)
            {
                return Required
                    ? new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} es requerido")
                    : ValidationResult.Success;
            }

            if (!decimal.TryParse(value.ToString(), out decimal DecimalToTest))
            {
                return new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} debe ser un número decimal válido");
            }

            if (DecimalToTest < MinimumDecimal || DecimalToTest > MaximumDecimal)
            {
                return new ValidationResult(
                    $"[{PropertyName}] El campo {NameToShowInUI} debe estar entre {MinimumDecimal} y {MaximumDecimal}");
            }

            return ValidationResult.Success;
        }
    }
}