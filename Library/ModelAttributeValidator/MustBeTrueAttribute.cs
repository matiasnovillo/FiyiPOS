using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Library.ModelAttributeValidator
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class MustBeTrueAttribute(string nameToShowInUI) : ValidationAttribute
    {
        private string NameToShowInUI { get; } = nameToShowInUI;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string PropertyName = validationContext.MemberName ?? "Field";

            if (value is not bool boolValue)
            {
                return new ValidationResult(
                    $"[{PropertyName}] {NameToShowInUI} debe ser un valor booleano válido.");
            }

            if (!boolValue)
            {
                return new ValidationResult(
                    $"[{PropertyName}] {NameToShowInUI} debe estar marcado como verdadero.");
            }

            return ValidationResult.Success;
        }
    }
}