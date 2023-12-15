using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Responses
{
    [ExcludeFromCodeCoverage]
    public class MarqetaPingResponse : Response
    {
        public string Version { get; set; }
        public string Timestamp { get; set; }
        public string Env { get; set; }
    }
}
