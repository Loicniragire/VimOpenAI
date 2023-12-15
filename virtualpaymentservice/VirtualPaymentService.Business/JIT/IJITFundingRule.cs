using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.JIT
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    public interface IJITFundingRule
    {
        JITDecision Execute(JITFundingRequest request, VCard vCard);
    }
}
