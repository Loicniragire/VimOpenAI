using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Requests.ValidationAttributes;

namespace VirtualPaymentService.Model.Requests
{
    /// <summary>
    /// Request contract for endpoint vcard/user/{userToken}
    /// </summary>
    /// <remarks>
    /// If property is not included JSON request it will not get updated at card provider.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class CardUserUpdateRequest
    {
        /// <summary>
        /// Telephone number of the user (including area code). Do not include hyphens, spaces, or parentheses.
        /// </summary>
        /// <remarks>
        /// Min length of the phone number is set to 4 to support more than US numbers. 
        /// </remarks>
        [RegularExpression("\\d*", ErrorMessage = "The phone number with area code must be passed in without hyphens, spaces, or parentheses.")]
        [StringLength(18, MinimumLength = 4, ErrorMessage = "The phone number provided must be between 4-18 digits. US phone numbers are 10 digits with area code.")]
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The country code of the phone number, (1 to 3) digit value, use 001 or 1 for US phone numbers.
        /// </summary>
        [RegularExpression("\\d*", ErrorMessage = "The country code must be numeric and passed in without hyphens, spaces, or parentheses.")]
        [StringLength(3, MinimumLength = 1, ErrorMessage = "The country code does not meet the required number of digits. The country code is a (1 to 3) digit value, use 001 or 1 for US phone numbers.")]
        [Required]
        public string PhoneNumberCountryCode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        /// <summary>
        /// The Country code where the customer resides. Use US for USA
        /// </summary>
        [RegularExpression("^$|[a-zA-Z]{2}$", ErrorMessage = "The country code must be two characters. Use a valid ISO alpha-2 country code. (e.g. US for USA)")]
        [StringLength(2, MinimumLength = 0, ErrorMessage = "The country code does not meet the required number of characters. The country code is a (2) character value, use a valid ISO alpha-2 country code. (e.g. US for USA)")]
        [CountryCode]
        public string CountryCode { get; set; } 
    }
}
