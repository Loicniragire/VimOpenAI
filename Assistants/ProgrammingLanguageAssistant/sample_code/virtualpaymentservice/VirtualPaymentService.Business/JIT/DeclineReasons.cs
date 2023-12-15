using System.Collections.Generic;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.JIT
{
    public static class DeclineReasons
    {
        private static readonly Dictionary<JITFundingDeclineReason, string> DeclineReasonLookup =
            new Dictionary<JITFundingDeclineReason, string>()
            {
                { JITFundingDeclineReason.None, "" },
                { JITFundingDeclineReason.CardExpired, "UserTransactionTime is greater than ActiveToDate" },
                { JITFundingDeclineReason.AmountMismatch, "Amount does not equal Invoice Amount" },
                { JITFundingDeclineReason.AmountTooHigh, "Amount is greater than Available Balance" },
                { JITFundingDeclineReason.InvalidCardStatus, "Card is not in an appropriate status to be authorized." },
                { JITFundingDeclineReason.InvalidTransactionType, "Unsupported transaction type." },
                { JITFundingDeclineReason.LeaseAlreadyFunded, "Lease already in Funded status" },
                { JITFundingDeclineReason.StateMismatch, "State does not match the lease State" },
                { JITFundingDeclineReason.StateMissing, "State missing from lease or vendor" },
                { JITFundingDeclineReason.PreviouslyAuthorized, "Card has already been used/authorized" }
            };

        public static string GetText(JITFundingDeclineReason reason)
        {
            return DeclineReasonLookup.GetValueOrDefault(reason, string.Empty);
        }
    }
}
