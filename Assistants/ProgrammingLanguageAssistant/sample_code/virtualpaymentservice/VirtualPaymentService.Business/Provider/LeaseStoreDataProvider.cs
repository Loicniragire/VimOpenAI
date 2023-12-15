using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Data;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Provider
{
    [Obsolete("This is a temporary solution for pulling lease/store data until VPO is able to provide it. Remove as soon as possible.")]
    public class LeaseStoreDataProvider : ILeaseStoreDataProvider
    {
        private readonly ILeaseStoreRepository _repository;

        public LeaseStoreDataProvider(ILeaseStoreRepository repository)
        {
            _repository = repository;
        }

        public LeaseStoreDataResponse GetLeaseStoreData(LeaseStoreDataRequest request)
        {
            var validation = _repository.GetDataForAuthorizationByLeaseId(request.LeaseId, request.ProviderCardId);

            if (validation == null)
            {
                return new LeaseStoreDataResponse
                {
                    Success = false,
                    Message = $"Lease/Store data not found for LeaseId {request.LeaseId} and ProviderCardId {request.ProviderCardId}"
                };
            }

            return new LeaseStoreDataResponse
            {
                Success = true,
                InvoiceAmount = validation.InvoiceAmount,
                IsMinAmountRequired = validation.IsMinAmountRequired,
                LeaseStatus = validation.LeaseStatus,
                State = validation.State,
                UseStateValidation = validation.UseStateValidation
            };
        }

        public MarqetaStateResponse GetMarqetaStateAbbreviation(int stateId)
        {
            var state = _repository.GetStateForValidationByMarqetaStateId(stateId);

            if (state == null)
            {
                return new MarqetaStateResponse
                {
                    Success = false,
                    Message = $"State abbreviation not found for StateId {stateId}"
                };
            }

            return new MarqetaStateResponse
            {
                Success = true,
                MarqetaStateId = state
            };
        }

        public async Task<IEnumerable<StoreCustomField>> GetStoreCustomFields(int leaseId)
        {
            return await _repository.GetStoreCustomFields(leaseId);
        }
    }
}
