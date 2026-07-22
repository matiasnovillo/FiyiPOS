using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Library.ModelAttributeValidator
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class MustBeFalseAttribute(string nameToShowInUI) : ValidationAttribute
    {
        private string nameToShowInUI { get; } = nameToShowInUI;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string PropertyName = validationContext.MemberName ?? "Field";

            if (value is not bool BoolValueToTest)
            {
                return new ValidationResult(
                    $"[{PropertyName}] {nameToShowInUI} debe ser un valor booleano válido.");
            }

            if (BoolValueToTest)
            {
                return new ValidationResult(
                    $"[{PropertyName}] {nameToShowInUI} debe estar marcado como falso.");
            }

            return ValidationResult.Success;
        }
    }
}