using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses.Marqeta;

namespace VirtualPaymentService.Model.DTO.Marqeta
{
    /// <summary>
    /// Used to help with mapping the request and the card created to save to the data repository. 
    /// </summary>
    public class MarqetaCardVirtualCard
    {
        public VirtualCardRequest CardRequest { get; set; }

        public MarqetaCardResponse CardCreated { get; set; }
    }
}
