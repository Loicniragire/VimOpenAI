using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Interface
{
    public interface ILeaseStoreDataProvider
    {
        LeaseStoreDataResponse GetLeaseStoreData(LeaseStoreDataRequest request);
        MarqetaStateResponse GetMarqetaStateAbbreviation(int stateId);

        Task<IEnumerable<StoreCustomField>> GetStoreCustomFields(int leaseId);
    }
}
