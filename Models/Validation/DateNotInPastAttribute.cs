using System.ComponentModel.DataAnnotations;

namespace ToDoListWebApp.Models.Validation
{
    public class DateNotInPastAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null) // Car 'DateLimite 'est un champ optionnel
            {
                return ValidationResult.Success;
            }

            if (value is DateOnly dateValue)
            {
                if (dateValue < DateOnly.FromDateTime(DateTime.Now))
                {
                    return new ValidationResult("La date limite ne peut pas être antérieure à la date du jour.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
