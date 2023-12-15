using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Model.DTO
{
    [ExcludeFromCodeCoverage]
    public class AddVCardAuthorizationResult
    {
        public AddVCardAuthorizationResult() { }

        public AddVCardAuthorizationResult(VCardPurchaseAuthResponse response, bool isDuplicate)
        {
            Response = response;
            SoftFail = isDuplicate;
        }

        /// <summary>
        /// <see cref="VCardPurchaseAuthResponse"/>
        /// </summary>
        public VCardPurchaseAuthResponse Response { get; set; }

        /// <summary>
        /// <see cref="bool"/> reflecting if the transaction was a duplicate.
        /// </summary>
        public bool SoftFail { get; set; }
    }
}
