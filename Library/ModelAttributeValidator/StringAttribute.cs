using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FiyiPOS.Library.ModelAttributeValidator
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class StringAttribute(string nameToShowInUI, bool required, long minimumLength, long maximumLength, string pattern) : ValidationAttribute
    {
        public string NameToShowInUI { get; } = nameToShowInUI;
        public bool Required { get; } = required;
        public long MinimumLength { get; } = minimumLength;
        public long MaximumLength { get; } = maximumLength;
        public string Pattern { get; } = pattern;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string PropertyName = validationContext.MemberName ?? "Field";

            if (value == null)
            {
                return Required
                    ? new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} es requerido.")
                    : ValidationResult.Success;
            }

            string StringToTest = value.ToString() ?? string.Empty;

            if (Required && string.IsNullOrWhiteSpace(StringToTest))
            {
                return new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} es requerido.");
            }

            if (!Required && string.IsNullOrWhiteSpace(StringToTest))
            {
                return ValidationResult.Success;
            }

            if (StringToTest.Length < MinimumLength || StringToTest.Length > MaximumLength)
            {
                return new ValidationResult(
                    $"[{PropertyName}] El campo {NameToShowInUI} debe tener entre {MinimumLength} y {MaximumLength} caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(Pattern) && !Regex.IsMatch(StringToTest, Pattern))
            {
                return new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} no cumple con el formato esperado.");
            }

            return ValidationResult.Success;
        }
    }
}