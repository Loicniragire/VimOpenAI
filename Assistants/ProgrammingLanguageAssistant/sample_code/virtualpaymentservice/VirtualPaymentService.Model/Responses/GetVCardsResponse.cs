using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Domain;

namespace VirtualPaymentService.Model.Responses
{
    [ExcludeFromCodeCoverage]
    public class GetVCardsResponse
    {
        public IList<VCard> VCards { get; set; }

        // TODO: Add properties to allow for pagination.
    }
}
