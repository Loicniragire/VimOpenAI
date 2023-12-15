using System;
using System.Collections.Generic;

namespace VirtualPaymentService.Model.Responses
{
    /// <summary>
    /// Authorizations for a virtual card.
    /// </summary>
    public class GetVirtualCardAuthorizationsResponse
    {
        public IList<VirtualCardAuthorization> Authorizations { set; get; } = new List<VirtualCardAuthorization>();
    }

    public class VirtualCardAuthorization
    {
        /// <summary>
        /// The date and time of the authorization.
        /// </summary>
        public DateTime AuthorizationDateTime { get; set; }

        /// <summary>
        /// The amount of the authorization.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The approval code of the authorization.
        /// </summary>
        public string ApprovalCode { get; set; }

        /// <summary>
        /// The response type of the authorization usually value of Approval or Decline.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// The type of the authorization.
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// The additional description of the <see cref="TypeCode"/>
        /// </summary>
        public string TypeDesc { get; set; }

        /// <summary>
        /// If the authorization was declined, the reason code.
        /// </summary>
        public string DeclineReasonCode { get; set; }

        /// <summary>
        /// If the authorization was declined, the reason message.
        /// </summary>
        public string DeclineReasonMessage { get; set; }

        /// <summary>
        /// The name of the merchant where the authorization originated.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// The merchant category code (MCC) of the merchant that submitted the authorization.
        /// </summary>
        public string MerchantCategoryCode { get; set; }
    }
}
