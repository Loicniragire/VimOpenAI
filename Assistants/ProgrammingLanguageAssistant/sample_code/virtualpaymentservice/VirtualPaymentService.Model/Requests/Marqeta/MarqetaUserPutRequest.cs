using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Requests.Marqeta
{
    /// <summary>
    /// Marqeta User PUT contract https://www.marqeta.com/docs/core-api/users#_update_user
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaUserPutRequest
    {
        /// <summary>
        /// Telephone number of the user (including area code), prepended by the 
        /// + symbol and the 1- to 3-digit country calling code. Do not include 
        /// hyphens, spaces, or parentheses.
        /// </summary>
        /// <remarks>
        /// Allowable Values Format: +15105551212 or +35552260859  (20 char max)
        /// </remarks>
        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("first_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LastName { get; set; }

        [JsonPropertyName("address1")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Address1 { get; set; }

        [JsonPropertyName("address2")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Address2 { get; set; }

        [JsonPropertyName("city")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string City { get; set; }

        [JsonPropertyName("state")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string State { get; set; }

        [JsonPropertyName("postal_code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PostalCode { get; set; }

        [JsonPropertyName("account_holder_group_token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AccountHolderGroupToken { get; set; } = null;
        
        [JsonPropertyName("country")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string CountryCode { get; set; }
    }
}
