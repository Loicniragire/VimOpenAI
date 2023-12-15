using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO.Marqeta
{

    /// <summary>
    /// Contains information related to the device being provisioned. This object is returned when relevant information 
    /// is provided by the token service provider, typically for device-based digital wallets such as GooglePay and ApplePay only.
    /// Reference from Marqeta https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_device_object_response 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaDigitalWalletTokenDevice
    {
        /// <summary>
        /// The unique identifier of the device object. This value (along with the subsequent data block) is returned if it is available for a given digital wallet token. 
        /// It is typically available for GooglePay, SamsungPay and ApplePay but not for ecommerce token requestors such as Paypal.
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// The type of device being provisioned.
        /// </summary>
        /// <remarks>
        /// Allowable Values: MOBILE_PHONE, WATCH, TABLET, MOBILE_PHONE_OR_TABLET, VEHICLE, APPLIANCE, LAPTOP, GAMING_DEVICE, UNKNOWN
        /// </remarks>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The language the device is configured to use. For example: eng
        /// </summary>
        [JsonPropertyName("language_code")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// The identity number of the device. This value is returned if it is available for a given digital wallet token.
        /// </summary>
        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }

        /// <summary>
        /// The device’s telephone number. This value is returned as provided by the digital wallet. 
        /// This value may be the full phone number, or just the last four digits.
        /// </summary>
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The name of the device. This value is returned as provided by the digital wallet.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The geographic coordinates of the device. This value is returned as provided by the digital wallet.
        /// </summary>
        /// <remarks>
        /// Allowable Values: DDD.DD/DDD.DD latitude/longitude
        /// NOTE: Both the longitude and latitude are prefixed with either a + or - symbol, for example: +42.29/-71.07
        /// </remarks>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        /// <summary>
        /// The device’s IP address. This value is returned as provided by the digital wallet.
        /// </summary>
        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; }
    }
}
