using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.JIT.Rules
{
    public class AvailableBalanceRule : IJITFundingRule
    {
        public JITDecision Execute(JITFundingRequest request, VCard vCard)
        {
            var minAuthAmount = 0M;
            var decision = new JITDecision();

            if (request.IsMinAmountRequired)
            {
                // Allow only one auth for a card with IsMinAmountRequired
                if (vCard.VCardStatusId == CardStatus.Authorized)
                {
                    decision.Approved = false;
                    decision.DeclineReason = JITFundingDeclineReason.PreviouslyAuthorized;

                    return decision;
                }

                // Should only be above $0.00 if MinAmount is required.
                minAuthAmount = vCard.OriginalCardBaseAmount - vCard.MaxAmountLess;
            }

            // Check if auth should be declined
            if (request.TransactionAmount < minAuthAmount ||
                request.TransactionAmount > vCard.AvailableBalance)
            {
                decision.Approved = false;

                // If isMinAmount required is false, AmountTooHigh is the only reason we would decline.
                decision.DeclineReason = request.IsMinAmountRequired
                    ? JITFundingDeclineReason.AmountMismatch
                    : JITFundingDeclineReason.AmountTooHigh;

                return decision;
            }

            decision.Approved = true;
            return decision;
        }
    }
}
