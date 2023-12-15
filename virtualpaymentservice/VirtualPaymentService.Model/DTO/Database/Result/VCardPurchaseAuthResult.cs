using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Model.DTO.Database.Result
{
    /// <summary>
    /// Class that handles the result of calling the database procedure 
    /// vpay.VCardPurchaseAuth_Get_ByVCardId in the Progressive database.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VCardPurchaseAuthResult : VirtualCardAuthorization
    {
        /// <summary>
        /// If the authorization was declined via JIT Funding, the Progressive internal reason message.
        /// </summary>
        public string ProgressiveDeclineReasonMessage { get; set; }

        /// <summary>
        /// The merchant category code MCC of the authorization
        /// </summary>
        public string CategoryCode { get; set; }
    }
}
