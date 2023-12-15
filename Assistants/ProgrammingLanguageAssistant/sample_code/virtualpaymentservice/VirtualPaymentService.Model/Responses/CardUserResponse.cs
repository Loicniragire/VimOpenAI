using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Responses
{
    public class CardUserResponse : CardUserUpdateResponse
    {
        /// <summary>
        /// The phone number set for the user on the card provider network.
        /// </summary>
        /// <remarks>
        /// This is overridden from the inherited <see cref="CardUserUpdateResponse"/> because we do not want
        /// to rendered in the JSON response if we did not set this property on the network.
        /// </remarks>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public new string PhoneNumber { get; set; }
    }
}
