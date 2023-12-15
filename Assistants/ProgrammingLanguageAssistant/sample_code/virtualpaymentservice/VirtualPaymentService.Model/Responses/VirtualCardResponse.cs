using System.ComponentModel.DataAnnotations;
using VirtualPaymentService.Model.Domain;

namespace VirtualPaymentService.Model.Responses
{
    /// <summary>
    /// Response contract for endpoint POST /vcard
    /// </summary>
    public class VirtualCardResponse
    {
        /// <summary>
        /// The ID of the lease related to the card.
        /// </summary>
        [Required]
        public int LeaseId { get; set; }

        [Required]
        public VCard Card { get; set; }

        /// <summary>
        /// The user information related to the card.
        /// </summary>
        public CardUserUpdateResponse User { get; set; }
    }
}