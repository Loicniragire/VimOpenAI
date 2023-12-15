using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Data.DynamicQueries;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Data
{
    public interface IVCardRepository
    {
        /// <summary>
        /// Marks a VCard as canceled.
        /// </summary>
        /// <param name="card">The <see cref="VCard"/> to cancel.</param>
        /// <returns><see cref="int"/> of how many rows were affected.</returns>
        Task<int> CancelVCardAsync(CancelVCardRequest card);

        /// <summary>
        /// Gets the stored details of a VCard.
        /// </summary>
        /// <param name="leaseId">The lease related to the VCard.</param>
        /// <param name="providerCardId">The unique identifier of the card (ReferenceId in the DB).</param>
        /// <returns><see cref="VCard"/></returns>
        Task<VCard> GetVCardAsync(long leaseId, string providerCardId);

        /// <summary>
        /// Asynchronously gets all VCards for a given lease.
        /// </summary>
        /// <param name="leaseId">The LeaseId</param>
        /// <returns><see cref="IEnumerable{}"/> of T <see cref="VCard"/></returns>
        Task<IEnumerable<VCard>> GetVCardsByLeaseAsync(long leaseId);

        /// <summary>
        /// Gets all canceled vcards with an UpdatedDate >= the date provided
        /// </summary>
        /// <param name="localDateTime"></param>
        /// <returns>Task of <see cref="CancelledVCardListResponse"/></returns>
        Task<IList<CancelledVCard>> GetCancelledVCardsByUpdatedDateAsync(DateTime localDateTime);

        /// <summary>
        /// Inserts a new virtual card into the data repository.
        /// </summary>
        /// <param name="card">The new <see cref="VCard"/> created to insert.</param>
        /// <returns>Task of <see cref="int"/> internal VCard ID.</returns>
        Task<int> InsertVCard(VCard card);

        /// <summary>
        /// Gets the total of authorization approvals, by 'vcardProviderId', that have occurred since 'startDate'
        /// </summary>
        /// <param name="vcardProviderId">The vcard provider Id to pull auth totals for.</param>
        /// <param name="startDate">The date, at which, to begin totalling auths from. Spans up to DateTime.Now</param>
        /// <returns>Task of <see cref="VCardProviderCreditLimit"/>POCO for auth total and creditLimit, returned by proc</returns>
        Task<VCardProviderCreditLimit> GetAuthTotalByProviderIdAsync(int vcardProviderId, DateTime startDate);

        /// <summary>
        /// Retrieves a vcard by its VCardId.
        /// </summary>
        /// <param name="vCardId">The primary id of the vcard.</param>
        /// <returns><see cref="VCard"/></returns>
        Task<VCard> GetVCardByIdAsync(int vCardId);

        /// <summary>
        /// Updates a vcard, using the provided params.
        /// </summary>
        /// <param name="sqlParams"></param>
        /// <returns><see cref="int"/> reflecting the number of rows affected.</returns>
        Task<int> UpdateVCardAsync(VCardUpdateParams sqlParams);

        /// <summary>
        /// Gets a filtered list of VCards, using the <see cref="GetVCardsDynamicQuery"/> to build the query.
        /// </summary>
        /// <param name="queryBuilder"></param>
        /// <returns>A list of <see cref="VCard"/></returns>
        Task<IList<VCard>> GetFilteredVCardsAsync(GetVCardsDynamicQuery queryBuilder);
    }
}
