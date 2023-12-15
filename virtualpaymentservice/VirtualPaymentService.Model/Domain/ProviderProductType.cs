using Dapper.Contrib.Extensions;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Domain
{
    /// <summary>
    /// The table responsible for listing the <see cref="ProductType"/>/s the <see cref="VirtualCardProviderNetwork"/> supports.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Table("VCard.ProviderProductType")]
    public class ProviderProductType
    {
        /// <summary>
        /// The PK of the table the ID is not used for anything here to make it easy with Dapper.
        /// </summary>
        [ExplicitKey]
        public int ProviderProductTypeId { get; set; }

        /// <summary>
        /// The provider that we are mapping a product to.
        /// </summary>
        /// <remarks>
        /// The combination of <see cref="ProviderId"/> and <see cref="ProductTypeId"/> is unique in the table.
        /// </remarks>
        public VirtualCardProviderNetwork ProviderId { get; set; }

        /// <summary>
        /// The product mapped to the provider.
        /// </summary>
        /// <remarks>
        /// The combination of <see cref="ProviderId"/> and <see cref="ProductTypeId"/> is unique in the table.
        /// </remarks>
        public ProductType ProductTypeId { get; set; }

        /// <summary>
        /// Soft delete marker.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Marks the default <see cref="ProductTypeId"/> for the <see cref="ProviderId"/>.
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
