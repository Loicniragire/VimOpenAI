using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;

namespace VirtualPaymentService.Data
{
    public interface ILeaseStoreRepository
    {
        JITFundingValidation GetDataForAuthorizationByLeaseId(int leaseId, string providerCardId);
        string GetStateForValidationByMarqetaStateId(int marqetaStateId);

        /// <summary>
        /// Asynchronously gets all Store Custom Fields for the store associated with a given lease.
        /// </summary>
        /// <param name="leaseId">The LeaseId</param>
        /// <returns><see cref="IEnumerable{}"/> of T <see cref="StoreCustomField"/></returns>
        Task<IEnumerable<StoreCustomField>> GetStoreCustomFields(int leaseId);
    }
}
