using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Responses.Marqeta
{
    /// <summary>
    /// Response Content for Marqeta errors.
    /// </summary>
    public class MarqetaHttpErrorResponse
    {
        /// <summary>
        /// Marqeta defined ErrorCode field
        /// </summary>
        [JsonPropertyName("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Marqeta description of ErrorCode.
        /// </summary>
        [JsonPropertyName("error_message")]
        public string ErrorMessage { get; set; }
    }
}
