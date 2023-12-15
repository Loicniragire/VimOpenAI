using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Responses.Marqeta
{
    /// <summary>
    /// Marqeta User response contract https://www.marqeta.com/docs/core-api/users#_retrieve_user
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaUserResponse
    {
        /// <summary>
        /// The unique identifier of the user.
        /// </summary>
        /// <remarks>
        /// Marqeta states 36 char is max size.
        /// </remarks>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// User in active state on the Marqeta network.
        /// </summary>
        [JsonPropertyName("active")]
        public bool Active { get; set; }

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
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("address1")]
        public string Address1 { get; set; }

        [JsonPropertyName("address2")]
        public string Address2 { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        /// <summary>
        /// In production this is where zip code is returned.
        /// </summary>
        [JsonPropertyName("zip")]
        public string Zip { get; set; }

        /// <summary>
        /// In sandbox this is where zip code is returned.
        /// </summary>
        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Datetime when user was created at Marqeta.
        /// </summary>
        /// <remarks>
        /// Would usually be the same time the virtual card was created for the user.
        /// </remarks>
        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Datetime of the last modification for the user at Marqeta.
        /// </summary>
        /// <remarks>
        /// This would be different than created if the user was updated (phone/address/etc.)
        /// </remarks>
        [JsonPropertyName("last_modified_time")]
        public DateTime LastModifiedTime { get; set; }
        
        /// <summary>
        /// Country code of the user at Marqeta
        /// </summary>
        [JsonPropertyName("country")]
        public string CountryCode { get; set; }
    }
}
