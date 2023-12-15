using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.JIT.Rules
{
    public class CardStatusRule : IJITFundingRule
    {
        public JITDecision Execute(JITFundingRequest request, VCard vCard)
        {
            if (vCard.VCardStatusId == CardStatus.Open || vCard.VCardStatusId == CardStatus.Authorized)
            {
                return new JITDecision { Approved = true };
            }
            else
            {
                return new JITDecision
                {
                    Approved = false,
                    DeclineReason = JITFundingDeclineReason.InvalidCardStatus
                };
            }
        }
    }
}
