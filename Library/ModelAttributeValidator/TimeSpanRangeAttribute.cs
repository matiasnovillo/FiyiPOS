using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Library.ModelAttributeValidator
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class TimeSpanRangeAttribute(string nameToShowInUI, bool required, string minimumTime, string maximumTime) : ValidationAttribute
    {
        public string NameToShowInUI { get; } = nameToShowInUI;
        public TimeSpan MinimumTimeSpan { get; } = TimeSpan.Parse(minimumTime);
        public TimeSpan MaximumTimeSpan { get; } = TimeSpan.Parse(maximumTime);
        public bool Required { get; } = required;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string PropertyName = validationContext.MemberName ?? "Field";

            if (value == null)
            {
                return Required
                    ? new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} es requerido.")
                    : ValidationResult.Success;
            }

            TimeSpan TimeSpanToTest;

            if (value is TimeSpan TimeSpan)
            {
                TimeSpanToTest = TimeSpan;
            }
            else if (!TimeSpan.TryParse(value.ToString(), out TimeSpanToTest))
            {
                return new ValidationResult(
                    $"[{PropertyName}] El campo {NameToShowInUI} no es  válido.");
            }

            if (TimeSpanToTest < MinimumTimeSpan || TimeSpanToTest > MaximumTimeSpan)
            {
                return new ValidationResult(
                    $"[{PropertyName}] El campo {NameToShowInUI} debe estar entre {MinimumTimeSpan} y {MaximumTimeSpan}.");
            }

            return ValidationResult.Success;
        }
    }
}