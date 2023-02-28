using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ChatBeet.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DateGreaterThanAttribute : ValidationAttribute
{
    public DateGreaterThanAttribute(string dateToCompareToFieldName)
    {
        DateToCompareToFieldName = dateToCompareToFieldName;
    }

    private string DateToCompareToFieldName { get; set; }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime laterDate)
            return ValidationResult.Success!;

        var referencedProperty = validationContext.ObjectType.GetProperty(DateToCompareToFieldName);

        if (referencedProperty!.GetValue(validationContext.ObjectInstance, null) is not DateTime earlierDate)
            return ValidationResult.Success!;

        return laterDate > earlierDate 
            ? ValidationResult.Success! 
            : new ValidationResult($"{validationContext.DisplayName} must be after {GetDisplayName(referencedProperty)}.");

        string GetDisplayName(MemberInfo pi)
        {
            var attrib = pi.GetCustomAttributes<DisplayAttribute>().SingleOrDefault();
            return attrib?.Name ?? pi.Name;
        }
    }
}