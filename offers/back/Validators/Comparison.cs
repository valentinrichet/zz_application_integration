using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AiOffer.Validators
{
    public enum ComparisonType
    {
        LessThan,
        LessThanOrEqualTo,
        EqualTo,
        GreaterThan,
        GreaterThanOrEqualTo
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ComparisonAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        private readonly ComparisonType _comparisonType;

        public ComparisonAttribute(string comparisonProperty, ComparisonType comparisonType)
        {
            _comparisonProperty = comparisonProperty;
            _comparisonType = comparisonType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;

            if (value == default)
            {
                return ValidationResult.Success;
            }

            if (value.GetType() == typeof(IComparable))
            {
                throw new ArgumentException("value has not implemented IComparable interface");
            }

            var currentValue = (IComparable)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (currentValue == null || property == null)
            {
                return ValidationResult.Success;
            }

            var comparisonValue = property.GetValue(validationContext.ObjectInstance);

            if (comparisonValue == default)
            {
                return ValidationResult.Success;
            }

            if (comparisonValue.GetType() == typeof(IComparable))
            {
                throw new ArgumentException("Comparison property has not implemented IComparable interface");
            }

            if (!ReferenceEquals(value.GetType(), comparisonValue.GetType()))
            {
                throw new ArgumentException("The properties types must be the same");
            }

            bool compareToResult;

            switch (_comparisonType)
            {
                case ComparisonType.LessThan:
                    compareToResult = currentValue.CompareTo((IComparable)comparisonValue) >= 0;
                    break;
                case ComparisonType.LessThanOrEqualTo:
                    compareToResult = currentValue.CompareTo((IComparable)comparisonValue) > 0;
                    break;
                case ComparisonType.EqualTo:
                    compareToResult = currentValue.CompareTo((IComparable)comparisonValue) != 0;
                    break;
                case ComparisonType.GreaterThan:
                    compareToResult = currentValue.CompareTo((IComparable)comparisonValue) <= 0;
                    break;
                case ComparisonType.GreaterThanOrEqualTo:
                    compareToResult = currentValue.CompareTo((IComparable)comparisonValue) < 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return compareToResult ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
}
