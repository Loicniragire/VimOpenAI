using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VirtualPaymentService.Model.Requests.ValidationAttributes
{
    [ExcludeFromCodeCoverage]
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[] AllowableValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validationArr = AllowableValues ?? Array.Empty<string>();
            if (validationArr.Contains(value?.ToString()))
            {
                return ValidationResult.Success;
            }

            var msg = $"{value} is not a permitted value. Please use one of the following: {string.Join(", ", validationArr)}.";
            return new ValidationResult(msg);
        }
    }
}
