using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ChatBeet.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        public DateGreaterThanAttribute(string dateToCompareToFieldName)
        {
            DateToCompareToFieldName = dateToCompareToFieldName;
        }

        private string DateToCompareToFieldName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var laterDate = (DateTime)value;
            var referencedProperty = validationContext.ObjectType.GetProperty(DateToCompareToFieldName);

            var earlierDate = (DateTime)referencedProperty.GetValue(validationContext.ObjectInstance, null);

            if (laterDate > earlierDate)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"{validationContext.DisplayName} must be after {GetDisplayName(referencedProperty)}.");
            }

            string GetDisplayName(PropertyInfo pi)
            {
                var attrib = pi.GetCustomAttributes<DisplayAttribute>().SingleOrDefault();
                return attrib?.Name ?? pi.Name;
            }
        }
    }
}