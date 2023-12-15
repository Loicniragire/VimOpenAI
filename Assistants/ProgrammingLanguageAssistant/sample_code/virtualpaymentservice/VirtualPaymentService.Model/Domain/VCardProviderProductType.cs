using Dapper.Contrib.Extensions;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Domain
{
    /// <summary>
    /// The table responsible for tracking what the <see cref="ProductType"/> is for the virtual card that was created.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Table("VCard.VCardProviderProductType")]
    public class VCardProviderProductType
    {
        /// <summary>
        /// The unique identifier of the card in Progressive systems.
        /// </summary>
        [ExplicitKey]
        public int VCardId { get; set; }

        /// <summary>
        /// The CardToken/ReferenceId from the provider.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// The provider network the card was created on.
        /// </summary>
        public VirtualCardProviderNetwork ProviderId { get; set; }

        /// <summary>
        /// The product type of the card.
        /// </summary>
        public ProductType ProductTypeId { get; set; }
    }
}
