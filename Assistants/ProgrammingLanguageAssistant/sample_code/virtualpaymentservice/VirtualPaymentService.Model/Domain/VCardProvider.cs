using Dapper.Contrib.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Domain
{
    [ExcludeFromCodeCoverage]
    [Table("VCard.Provider")]
    public class VCardProvider
    {
        /// <summary>
        /// The primary identifier for the provider.
        /// </summary>
        [Key]
        public int ProviderId { get; set; }

        /// <summary>
        /// The name of the provider (e.g. Marqeta).
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// A boolean reflecting if the provider is currently in use at Progressive.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// An integer to reflect reflecting the preference we have towards using a given provider.
        /// The lower the number, the higher the preference. Sort of like golf.
        /// </summary>
        public int BusinessRank { get; set; }

        /// <summary>
        /// The max amount of credit that can be accrued with the given provider within the.
        /// <see cref="AuthExpireDays"/>
        /// </summary>
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// Number of days until auths expire and our credit limit refreshes.
        /// </summary>
        public int AuthExpireDays { get; set; }


        public decimal CreditLimitCutoffPercentage { get; set; }

        public bool CreditLimitReached { get; set; }

    }
}
