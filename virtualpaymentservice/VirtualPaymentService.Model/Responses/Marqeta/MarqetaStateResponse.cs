using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Responses
{
    [ExcludeFromCodeCoverage]
    public class MarqetaStateResponse : Response
    {
        public string MarqetaStateId { get; set; }
    }
}