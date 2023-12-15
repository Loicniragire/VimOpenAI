using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VirtualPaymentService.Data.DynamicQueries;
using VirtualPaymentService.Data.Interface;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Data.Facade
{
    [ExcludeFromCodeCoverage]
    public class VirtualPaymentRepository : IVirtualPaymentRepository
    {
        #region Field
        private readonly IVCardRepository _vCardRepo;
        private readonly IWalletBatchConfigRepository _walletBatchConfigRepo;
        private readonly IProviderRepository _providerRepo;
        private readonly IVCardProviderProductTypeRepository _vCardProviderProductTypeRepo;
        #endregion Field

        #region Constructor
        public VirtualPaymentRepository(
            IVCardRepository vCardRepo,
            IWalletBatchConfigRepository walletBatchConfigRepo,
            IProviderRepository providerRepo,
            IVCardProviderProductTypeRepository vCardProviderProductTypeRepo)
        {
            _vCardRepo = vCardRepo;
            _walletBatchConfigRepo = walletBatchConfigRepo;
            _providerRepo = providerRepo;
            _vCardProviderProductTypeRepo = vCardProviderProductTypeRepo;
        }
        #endregion Constructor

        #region Method
        /// <inheritdoc />
        public Task<int> CancelVCardAsync(CancelVCardRequest card)
        {
            return _vCardRepo.CancelVCardAsync(card);
        }

        /// <inheritdoc />
        public Task<IList<CancelledVCard>> GetCancelledVCardsByUpdatedDateAsync(DateTime localDateTime)
        {
            return _vCardRepo.GetCancelledVCardsByUpdatedDateAsync(localDateTime);
        }

        /// <inheritdoc />
        public Task<IEnumerable<VCard>> GetVCardsByLeaseAsync(long leaseId)
        {
            return _vCardRepo.GetVCardsByLeaseAsync(leaseId);
        }

        /// <inheritdoc />
        public Task<int> InsertVCard(VCard card)
        {
            return _vCardRepo.InsertVCard(card);
        }

        /// <inheritdoc />
        public Task<VCard> GetVCardAsync(long leaseId, string providerCardId)
        {
            return _vCardRepo.GetVCardAsync(leaseId, providerCardId);
        }

        /// <inheritdoc />
        public Task<IEnumerable<VCardProvider>> GetVCardProvidersAsync()
        {
            return _providerRepo.GetVCardProvidersAsync();
        }

        /// <inheritdoc />
        public Task<StoreProductType> GetStoreProductTypeAsync(int storeId)
        {
            return _providerRepo.GetStoreProductTypeAsync(storeId);
        }

        /// <inheritdoc />
        public Task<IEnumerable<ProviderProductType>> GetProviderProductTypeAsync()
        {
            return _providerRepo.GetProviderProductTypeAsync();
        }

        /// <inheritdoc />
        public Task<WalletBatchConfig> GetWalletBatchConfigAsync()
        {
            return _walletBatchConfigRepo.GetWalletBatchConfigAsync();
        }

        /// <inheritdoc />
        public Task UpdateWalletBatchConfigAsync(UpdateWalletBatchConfigRequest request)
        {
            return _walletBatchConfigRepo.UpdateWalletBatchConfigAsync(request);
        }

        /// <inheritdoc />
        public Task<VCardProviderCreditLimit> GetAuthTotalByProviderIdAsync(int vcardProviderId, DateTime startDate)
        {
            return _vCardRepo.GetAuthTotalByProviderIdAsync(vcardProviderId, startDate);
        }

        /// <inheritdoc />
        public async Task<VCard> GetVCardByIdAsync(int vCardId)
        {
            return await _vCardRepo.GetVCardByIdAsync(vCardId);
        }

        /// <inheritdoc />
        public async Task<int> UpdateVCardAsync(VCardUpdateParams sqlParams)
        {
            return await _vCardRepo.UpdateVCardAsync(sqlParams);
        }

        public async Task InsertVCardProviderProductTypeAsync(VCardProviderProductType cardInfo)
        {
            await _vCardProviderProductTypeRepo.InsertVCardProviderProductTypeAsync(cardInfo);
        }

        /// <inheritdoc />
        public async Task<VCardProviderProductType> GetVCardProviderProductTypeAsync(string referenceId)
        {
            return await _vCardProviderProductTypeRepo.GetVCardProviderProductTypeAsync(referenceId);
        }

        /// <inheritdoc />
        public async Task<IList<VCard>> GetFilteredVCardsAsync(GetVCardsDynamicQuery queryBuilder)
        {
            return await _vCardRepo.GetFilteredVCardsAsync(queryBuilder);
        }
        #endregion Method
    }
}
