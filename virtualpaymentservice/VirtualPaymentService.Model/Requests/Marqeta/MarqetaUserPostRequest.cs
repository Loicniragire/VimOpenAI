using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Requests.Marqeta
{
    /// <summary>
    /// Marqeta User POST contract https://www.marqeta.com/docs/core-api/users#_create_user
    /// </summary>
    public class MarqetaUserPostRequest : MarqetaUserPutRequest
    {
        /// <summary>
        /// The unique identifier of the user.
        /// </summary>
        /// <remarks>
        /// We use the LeaseId as the unique identifier for a user at Marqeta. At this time we create
        /// a user for each lease and do not have a user that maps to a client in the Progressive systems.
        /// </remarks>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("metadata")]
        public MarqetaUserMetadata Metadata { get; set; }
    }

    /// <summary>
    /// Progressive injected metadata for the Marqeta user.
    /// </summary>
    /// <remarks>
    /// We can define up to 20 key value pairs, 255 char max per name and value.
    /// </remarks>
    public class MarqetaUserMetadata
    {
        /// <summary>
        /// LeaseId related to the card and user.
        /// </summary>
        public string LeaseId { get; set; }

        /// <summary>
        /// The store related to lease.
        /// </summary>
        public string StoreId { get; set; }
    }
}
