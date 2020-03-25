using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class TypeAttribute : ValidationAttribute
    {
        private readonly ICollection<string> types = new List<string>{ "PART-TIME", "FULL-TIME", "OTHER" };

        public TypeAttribute()
        { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;

            if (value == default)
            {
                return ValidationResult.Success;
            }

            if (value.GetType() != typeof(string))
            {
                throw new ArgumentException("value must be a string");
            }

            string currentValue = (string) value;

            return types.Any(level => String.Equals(currentValue, level, StringComparison.OrdinalIgnoreCase)) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
