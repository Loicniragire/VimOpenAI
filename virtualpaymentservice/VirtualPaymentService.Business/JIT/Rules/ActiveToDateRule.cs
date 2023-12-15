using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.JIT.Rules
{
    public class ActiveToDateRule : IJITFundingRule
    {
        public JITDecision Execute(JITFundingRequest request, VCard vCard)
        {
            var activeToDateUtc = vCard.ActiveToDate.ToUniversalTime();
            if (request.TransactionDate <= activeToDateUtc)
            {
                return new JITDecision { Approved = true };
            }
            else
            {
                return new JITDecision
                {
                    Approved = false,
                    DeclineReason = JITFundingDeclineReason.CardExpired
                };
            }
        }
    }
}
