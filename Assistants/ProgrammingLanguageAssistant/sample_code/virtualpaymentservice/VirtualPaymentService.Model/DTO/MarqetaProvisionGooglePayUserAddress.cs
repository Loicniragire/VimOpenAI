using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO
{
    public class MarqetaProvisionGooglePayUserAddress
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("address1")]
        public string Address1 { get; set; }

        [JsonPropertyName("address2")]
        public string Address2 { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// The response may contain a property that is named zip instead of postal_code.
        /// </summary>
        /// <remarks>
        /// It is unknown if the zip code is named postal_code or zip from Marqeta. This is 
        /// here to possibly capture the needed zip values from Marqeta.
        /// </remarks>
        [JsonPropertyName("zip")]
        public string ZipCode { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }
    }
}
