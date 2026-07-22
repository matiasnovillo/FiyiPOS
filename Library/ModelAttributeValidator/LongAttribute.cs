using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Library.ModelAttributeValidator
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class LongAttribute(string nameToShowInUI, bool required, long minimumLong, long maximumLong) : ValidationAttribute
    {
        public string NameToShowInUI { get; } = nameToShowInUI;
        public bool Required { get; } = required;
        public long MinimumLong { get; } = minimumLong;
        public long MaximumLong { get; } = maximumLong;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string PropertyName = validationContext.MemberName ?? "Field";

            if (value == null)
            {
                return Required
                    ? new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} es requerido.")
                    : ValidationResult.Success;
            }

            if (!long.TryParse(value.ToString(), out long NumberToTest))
            {
                return new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} debe ser numérico.");
            }

            if (NumberToTest < MinimumLong || NumberToTest > MaximumLong)
            {
                return new ValidationResult(
                    $"[{PropertyName}] El campo {NameToShowInUI} debe estar entre {MinimumLong} y {MaximumLong}.");
            }

            return ValidationResult.Success;
        }
    }
}