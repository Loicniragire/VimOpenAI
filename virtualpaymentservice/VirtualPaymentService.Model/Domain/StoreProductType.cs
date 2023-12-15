using Dapper.Contrib.Extensions;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Domain
{
    /// <summary>
    /// The table responsible for listing the <see cref="ProductType"/> a specific Store should use based on business requirements.
    /// If there is no mapping the default for the <see cref="VirtualCardProviderNetwork"/> is used.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Table("VCard.StoreProductType")]
    public class StoreProductType
    {
        /// <summary>
        /// The primary identifier for the store.
        /// </summary>
        [ExplicitKey]
        public int StoreId { get; set; }

        /// <summary>
        /// The <see cref="ProductType"/> that this store is set 
        /// to use that overrides the default. 
        /// </summary>
        public ProductType ProductTypeId { get; set; }

        /// <summary>
        /// Setting which describes whether or not the customer information should be tied to user/lease associated with virtual card.
        /// </summary>
        /// <remarks>
        /// true: vCard user details (name on card, address, phone, etc.) - to be set with Prog Client defined values, in provider system.
        /// false: vCard user details empty - Provider uses Progressive defining shipping address for card information, as default.
        /// </remarks>
        public bool CustomerInfoRequired { get; set; }
    }
}