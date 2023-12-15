using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.DTO
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    [ExcludeFromCodeCoverage]
    public class JITDecision
    {
        public bool Approved { get; set; }
        public JITFundingDeclineReason DeclineReason { get; set; }
    }
}
