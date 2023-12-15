using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.JIT.Rules
{
    public class FundedRule : IJITFundingRule
    {
        private const string LeaseStatusFunded= "FUNDED";

        public JITDecision Execute(JITFundingRequest request, VCard vCard)
        {
            if (request.LeaseStatus?.ToUpper() == LeaseStatusFunded && vCard.VCardStatusId == CardStatus.Authorized && request.IsMinAmountRequired)
            {
                return new JITDecision
                {
                    Approved = false,
                    DeclineReason = JITFundingDeclineReason.LeaseAlreadyFunded
                };
            }
            else
            {
                return new JITDecision { Approved = true };
            }
        }
    }
}
