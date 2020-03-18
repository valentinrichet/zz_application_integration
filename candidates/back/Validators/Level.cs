using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiCandidate.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class LevelAttribute : ValidationAttribute
    {
        private readonly ICollection<string> levels = new List<string>{ "L1", "L2", "L3", "M1", "M2", "D1", "D2", "+" };


        public LevelAttribute()
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

            return levels.Any(level => String.Equals(currentValue, level, StringComparison.OrdinalIgnoreCase)) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
