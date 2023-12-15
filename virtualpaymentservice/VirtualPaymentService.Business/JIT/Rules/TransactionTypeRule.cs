using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.JIT.Rules
{
    public class TransactionTypeRule : IJITFundingRule
    {
        private const string TransactionTypeAuthorization = "AUTHORIZATION";

        public JITDecision Execute(JITFundingRequest request, VCard vCard)
        {
            if (request.TransactionType?.ToUpper() == TransactionTypeAuthorization)
            {
                return new JITDecision { Approved = true };
            }
            else
            {
                return new JITDecision
                {
                    Approved = false,
                    DeclineReason = JITFundingDeclineReason.InvalidTransactionType
                };
            }
        }
    }
}
