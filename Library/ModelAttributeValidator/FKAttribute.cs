using System.ComponentModel.DataAnnotations;

namespace FiyiPOS.Library.ModelAttributeValidator
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class FKAttribute(string nameToShowInUI) : ValidationAttribute
    {
        public string NameToShowInUI { get; } = nameToShowInUI;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string PropertyName = validationContext.MemberName ?? "Field";

            if (value == null)
            {
                return new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} es obligatorio");
            }

            if (value is long FKValueToTest)
            {
                if (FKValueToTest <= 0)
                {
                    return new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} debe ser mayor a 0.");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult($"[{PropertyName}] El campo {NameToShowInUI} debe ser de tipo long.");
        }
    }
}