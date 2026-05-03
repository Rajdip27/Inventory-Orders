using System.ComponentModel.DataAnnotations;

namespace InventoryOrders.Application.Extensions;
/// <summary>
/// Provides a validation attribute that ensures a date value is later than the value of another specified date property
/// on the same object.
/// </summary>
/// <remarks>Use this attribute to enforce that one date property must be chronologically greater than another
/// property, such as ensuring an 'EndDate' is after a 'StartDate'. The comparison is performed using the property name
/// provided to the constructor. If the specified comparison property does not exist, an ArgumentException is thrown
/// during validation. This attribute is typically used in data annotation scenarios for model validation.</remarks>

public class DateGreaterThanAttribute(string comparisonProperty) : ValidationAttribute
{
    private readonly string _comparisonProperty = comparisonProperty;

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
		try
		{
            var currentValue = (DateTimeOffset?)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTimeOffset?)property.GetValue(validationContext.ObjectInstance);

            if (currentValue != null && comparisonValue != null && currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be later than {_comparisonProperty}.");


            return ValidationResult.Success;
        }
		catch (Exception ex)
		{
            Console.WriteLine($"Validation Result",ex.Message);
			throw;
		}
    }
}

