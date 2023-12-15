using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Interface
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    public interface IJITDecisionProvider
    {
        Task<JITFundingResponse> ProcessJITDecisionAsync(JITFundingRequest request);
    }
}
