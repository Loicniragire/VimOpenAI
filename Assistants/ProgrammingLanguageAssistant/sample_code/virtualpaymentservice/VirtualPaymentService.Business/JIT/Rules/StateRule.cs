using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.JIT.Rules
{
    public class StateRule : IJITFundingRule
    {
        public JITDecision Execute(JITFundingRequest request, VCard vCard)
        {
            var decision = new JITDecision();

            if (request.UseStateValidation)
            {
                if (string.IsNullOrEmpty(request.TransactionState) ||
                    string.IsNullOrEmpty(request.StoreAddressState))
                {
                    decision.Approved = false;
                    decision.DeclineReason = JITFundingDeclineReason.StateMissing;
                }
                else if (request.TransactionState.ToLower() == request.StoreAddressState.ToLower())
                {
                    decision.Approved = true;
                }
                else
                {
                    decision.Approved = false;
                    decision.DeclineReason = JITFundingDeclineReason.StateMismatch;
                }
            }
            else
            {
                decision.Approved = true;
            }

            return decision;
        }
    }
}
