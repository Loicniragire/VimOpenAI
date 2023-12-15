using System;
using System.Text.Json.Serialization;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Responses.Marqeta
{
    /// <summary>
    /// The response contract of the POST card endpoint: https://www.marqeta.com/docs/core-api/cards#_response_body
    /// </summary>
    public class MarqetaCardResponse
    {
        /// <summary>
        /// Datetime when card was created at Marqeta.
        /// </summary>
        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Datetime of the last modification for the card at Marqeta.
        /// </summary>
        [JsonPropertyName("last_modified_time")]
        public DateTime LastModifiedTime { get; set; }

        /// <summary>
        /// The unique identifier of card.
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// The unique identifier of the user related to the card.
        /// </summary>
        /// <remarks>
        /// The token for the user is usually the Progressive LeaseId.
        /// </remarks>
        [JsonPropertyName("user_token")]
        public string UserToken { get; set; }

        /// <summary>
        /// The unique identifier of the card product, configured on Progressive's 
        /// Marqeta account, that was created.
        /// </summary>
        [JsonPropertyName("card_product_token")]
        public string CardProductToken { get; set; }

        /// <summary>
        /// The last four digits of the card PAN.
        /// </summary>
        [JsonPropertyName("last_four")]
        public string LastFour { get; set; }

        /// <summary>
        /// The primary account number (PAN) of the card.
        /// </summary>
        [JsonPropertyName("pan")]
        public string CardNumber { get; set; }

        /// <summary>
        /// The expiration date in MMyy format.
        /// </summary>
        [JsonPropertyName("expiration")]
        public string Expiration { get; set; }

        /// <summary>
        /// The expiration date in MMyy format.
        /// </summary>
        [JsonPropertyName("expiration_time")]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// The three-digit card verification value (CVV2 or CVC2) printed on the card.
        /// </summary>
        [JsonPropertyName("cvv_number")]
        public string CVV { get; set; }

        /// <summary>
        /// The barcode printed on the card, expressed as numerals.
        /// </summary>
        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }

        /// <summary>
        /// Specifies if the personal identification number (PIN) has been set for the card.
        /// </summary>
        [JsonPropertyName("pin_is_set")]
        public bool IsPinSet { get; set; }

        /// <summary>
        /// Indicates the state of the card on the Marqeta network.
        /// </summary>
        [JsonPropertyName("state")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MarqetaCardState State { get; set; }

        /// <summary>
        /// A descriptive reason for the state of the card.
        /// </summary>
        [JsonPropertyName("state_reason")]
        public string StateReason { get; set; }

        /// <summary>
        /// The card fulfillment status.
        /// </summary>
        /// <remarks>
        /// Allowable Values: ISSUED, ORDERED, REORDERED, REJECTED, SHIPPED, DELIVERED, DIGITALLY_PRESENTED
        /// </remarks>
        [JsonPropertyName("fulfillment_status")]
        public string FulfillmentStatus { get; set; }

        /// <summary>
        /// The instrument type of the card.
        /// </summary>
        /// <remarks>
        /// Allowable Values: PHYSICAL_MSR, PHYSICAL_ICC, PHYSICAL_CONTACTLESS, PHYSICAL_COMBO, VIRTUAL_PAN
        /// </remarks>
        [JsonPropertyName("instrument_type")]
        public string InstrumentType { get; set; }

        /// <summary>
        /// A value of true indicates that you requested expedited processing of the card from your card fulfillment provider.
        /// </summary>
        [JsonPropertyName("expedite")]
        public bool Expedite { get; set; }
    }
}
