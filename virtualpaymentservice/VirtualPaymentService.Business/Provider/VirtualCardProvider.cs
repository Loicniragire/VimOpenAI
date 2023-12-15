using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Factory.Interface;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Service.Helpers;
using VirtualPaymentService.Data.DynamicQueries;
using VirtualPaymentService.Data.Facade;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Provider
{
    /// <summary>
    /// Provider that interacts with <see cref="ICardProvider"/>s.
    /// </summary>
    public class VirtualCardProvider : IVirtualCardProvider
    {
        #region Field
        private readonly ICardProviderFactory _providerFactory;
        private readonly IVirtualPaymentRepository _virtualPaymentRepo;
        private readonly IMapper _mapper;
        private readonly AppSettings _appsettings;
        #endregion Field

        #region Constructor
        public VirtualCardProvider
        (
            ICardProviderFactory providerFactory,
            IVirtualPaymentRepository virtualPaymentRepo,
            IMapper mapper,
            AppSettings appSettings
        )
        {
            _providerFactory = providerFactory;
            _virtualPaymentRepo = virtualPaymentRepo;
            _mapper = mapper;
            _appsettings = appSettings;
        }
        #endregion Constructor

        #region Method
        /// <inheritdoc />
        public async Task<bool> CancelVCardAsync(CancelVCardRequest request)
        {
            // Cancel the vcard on the vcard's provider network.
            var productType = await GetProductTypeByCardTokenAsync(request.ReferenceId, VirtualCardProviderNetwork.Marqeta);

            var providerCardCanceled = await _providerFactory.GetCardProvider(request.VCardProviderId).CancelCardAsync(request, productType);
            if (!providerCardCanceled)
            {
                return false;
            }

            // Persist cancellation to repo.
            // Will throw if there is an exception.
            await _virtualPaymentRepo.CancelVCardAsync(request);

            return true;
        }

        /// <inheritdoc />
        public async Task<ApplePayTokenizationResponse> GetApplePayTokenizationDataAsync(ApplePayTokenizationRequest request)
        {
            // This implementation is hardcoded for Marqeta, with the expectation that it will be changed.
            // Once we have other wallet providers to use for getting tokenization wallets,
            // We will likely implement more logic and include a waterfall in the event of an unsuccessful request.
            // Potential options: provider specific endpoints, passing ProviderId in request, or fetching most recent card again.
            IDigitalWalletProvider provider = _providerFactory.GetWalletProvider(VPaymentProvider.Marqeta);

            var productType = await GetProductTypeByCardTokenAsync(request.Data.CardToken, VirtualCardProviderNetwork.Marqeta);

            return await provider.GetApplePayTokenizationDataAsync(request.Data, productType);
        }

        /// <inheritdoc />
        public async Task<GooglePayTokenizationResponse> GetGooglePayTokenizationDataAsync(GooglePayTokenizationRequest request)
        {
            IDigitalWalletProvider provider = _providerFactory.GetWalletProvider(VPaymentProvider.Marqeta);

            var productType = await GetProductTypeByCardTokenAsync(request.Data.CardToken, VirtualCardProviderNetwork.Marqeta);

            return await provider.GetGooglePayTokenizationDataAsync(request.Data, productType);
        }

        /// <inheritdoc/>
        public async Task<DigitalWalletTokenResponse> GetDigitalWalletTokensByVCardAsync(string cardToken)
        {
            IDigitalWalletProvider provider = _providerFactory.GetWalletProvider(VPaymentProvider.Marqeta);

            var productType = await GetProductTypeByCardTokenAsync(cardToken, VirtualCardProviderNetwork.Marqeta);
            return await provider.GetDigitalWalletTokensByVCardAsync(cardToken, productType);
        }

        /// <inheritdoc />
        public async Task<GetVCardsResponse> GetVCardsByFilterAsync(GetVCardsRequest queryParams)
        {
            var vCards = await _virtualPaymentRepo.GetFilteredVCardsAsync(_mapper.Map<GetVCardsDynamicQuery>(queryParams));

            // If vcard 'productId' ever required for this method, or associated endpoint, simply call private method: SetVirtualCardProductId(vCards)
            // Note: The repository for this call does not order the results, as is done in the sProc utilized for:
            // _virtualPaymentRepo.GetVCardsByLeaseAsync(leaseId), where results ordered by VCardId descending.

            return new GetVCardsResponse() { VCards = vCards };
        }

        /// <inheritdoc/>
        public async Task<DigitalWalletTokenTransitionResponse> TransitionDigitalWalletTokenAsync(DigitalWalletTokenTransitionRequest request)
        {
            IDigitalWalletProvider provider = _providerFactory.GetWalletProvider(VPaymentProvider.Marqeta);

            var productType = await GetProductTypeByCardTokenAsync(request.DigitalWalletToken.CardToken, VirtualCardProviderNetwork.Marqeta);

            return await provider.TransitionDigitalWalletTokenAsync(request, productType);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<VCard>> GetVCardsByLeaseAsync(long leaseId)
        {
            var vCards = await _virtualPaymentRepo.GetVCardsByLeaseAsync(leaseId);

            await SetVirtualCardProductId(vCards);

            return vCards;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VCardProvider>> GetVCardProvidersAsync(bool activeOnly)
        {
            var providers = await _virtualPaymentRepo.GetVCardProvidersAsync();

            if (activeOnly)
            {
                providers = providers.Where(p => p.IsActive);
            }

            // Check if CreditLimit has been reached.
            foreach (var provider in providers.Where(p => p.CreditLimit > 0))
            {
                var creditResult = await _virtualPaymentRepo.GetAuthTotalByProviderIdAsync(provider.ProviderId,
                    DateUtil.GetStartDateByAuthExpireDays(provider.AuthExpireDays, DateTime.Now));

                provider.CreditLimitReached = creditResult.TotalAuthAmount > (provider.CreditLimit * provider.CreditLimitCutoffPercentage);
            }

            return providers;
        }

        /// <inheritdoc/>
        public async Task<CardUserUpdateResponse> UpdateCardUserAsync(string userToken, CardUserUpdateRequest request)
        {
            // Get all Marqeta card product types. When integrate with future providers,
            // ProviderId may be passed in request, fetched by most recent card, or have provider explicit endpoint(s).
            IDigitalWalletProvider provider = _providerFactory.GetWalletProvider(VPaymentProvider.Marqeta);

            var providerProductType = await _virtualPaymentRepo.GetProviderProductTypeAsync();
            var networkProductTypes = providerProductType.Where(p => p.ProviderId == VirtualCardProviderNetwork.Marqeta).ToList();

            // Configure Tasks for attempting to update user for any/all card products.
            Task userUpdateAggregateTasks = null;
            bool allProductsUpdateFailed = false;

            var updateUserTasks = networkProductTypes
                .DistinctBy(p => p.ProductTypeId)
                .Select(p => provider.UpdateCardUserAsync(userToken, request, p.ProductTypeId))
                .ToList();

            try
            {
                // Fire and await updateUserTasks
                userUpdateAggregateTasks = Task.WhenAll(updateUserTasks);
                await userUpdateAggregateTasks;
            }
            catch (Exception)
            {
                // If fail out on network for all card products, set local variable
                if (userUpdateAggregateTasks?.Exception?.InnerExceptions != null &&
                    userUpdateAggregateTasks.Exception.InnerExceptions.Count == networkProductTypes.Count)
                {
                    allProductsUpdateFailed = true;
                }
            }

            // Check for any successful update results. 
            var updateResults = updateUserTasks.FirstOrDefault(r => r.IsCompletedSuccessfully && r.Result != null);

            // If update attempts, for all card products failed, return the first caught original Aggregate InnerException
            // (This would typically be the first 'HttpRequestException' caught/thrown by MarqetaProvider.UpdateCardUserAsync)
            // Otherwise throw any other misc. exception which could have occurred.
            if (updateResults == null || allProductsUpdateFailed)
            {
                throw userUpdateAggregateTasks?.Exception?.InnerException ??
                      new AggregateException($"Unknown exception occurred while attempting to update userToken: {userToken}");
            }

            // Found at least one successful update, so return results.
            return updateResults.Result;
        }

        /// <inheritdoc/>
        public async Task<CardUserResponse> GetCardUserAsync(string userToken, VirtualCardProviderNetwork cardProvider, ProductType productType)
        {
            return await _providerFactory.GetCardProvider(cardProvider).GetCardUserAsync(userToken, productType);
        }

        /// <inheritdoc/>
        public async Task<WalletBatchConfigResponse> GetWalletBatchConfigAsync()
        {
            var configModel = await _virtualPaymentRepo.GetWalletBatchConfigAsync();

            return _mapper.Map<WalletBatchConfigResponse>(configModel);
        }

        /// <inheritdoc />
        public Task UpdateWalletBatchConfigAsync(UpdateWalletBatchConfigRequest request)
        {
            return _virtualPaymentRepo.UpdateWalletBatchConfigAsync(request);
        }

        /// <inheritdoc />
        public Task<IList<CancelledVCard>> GetCancelledVCardsByDateTimeAsync(DateTimeOffset lookupDateTime)
        {
            // The UpdatedDate column is in MST, not UTC
            var mstZoneId = "Mountain Standard Time";
            var mstZone = TimeZoneInfo.FindSystemTimeZoneById(mstZoneId);
            var mstTime = TimeZoneInfo.ConvertTime(lookupDateTime, mstZone).DateTime;

            return _virtualPaymentRepo.GetCancelledVCardsByUpdatedDateAsync(mstTime);
        }

        /// <inheritdoc />
        public async Task<VirtualCardResponse> CreateCardAsync(VirtualCardRequest request)
        {
            var productType = (await ProductTypeByProviderStoreAsync(request.VCardProviderId, (int)request.StoreId)).ProductTypeId;

            ICardProvider provider = _providerFactory.GetCardProvider(request.VCardProviderId);

            // Creates the card at the provider
            var newCard = await provider.CreateCardAsync(request, productType);

            // Saves the card to the data repository
            _ = await _virtualPaymentRepo.InsertVCard(newCard.Card);

            // Get the saved card from the repository
            var savedCard = await _virtualPaymentRepo.GetVCardAsync(newCard.LeaseId, newCard.Card.ReferenceId);

            // This is not available from the prior call, required for subsequent insert and cardUser.
            savedCard.ProductTypeId = productType;

            // Save additional VCard info
            await _virtualPaymentRepo.InsertVCardProviderProductTypeAsync(_mapper.Map<VCardProviderProductType>(savedCard));

            // Take the retrieved card info and replace the card info we had after creation at the provider.  
            // This needs to be done so all of the values for VCard are populated.
            // Some values/columns are calculated in the DB table/column default values.
            newCard.Card = savedCard;

            return newCard;
        }

        /// <inheritdoc />
        public async Task<bool> IsRequiredUserPhoneMissingAsync(VirtualCardRequest request)
        {
            var productType = (await ProductTypeByProviderStoreAsync(request.VCardProviderId, (int)request.StoreId)).ProductTypeId;
            var isPinRequired = _appsettings.GetCardProviderSetting(request.VCardProviderId, productType).SetInitalPin;

            // The initial pin is the last 4 of the phone number.
            if (isPinRequired && string.IsNullOrEmpty(request.User?.PhoneNumber))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public async Task<StoreProductType> GetProductTypeAsync(VirtualCardProviderNetwork providerId, int storeId)
        {
            return await ProductTypeByProviderStoreAsync(providerId, storeId);
        }
        #endregion Method

        #region private helper methods

        private async Task<ProductType> GetProductTypeByCardTokenAsync(string cardToken,
            VirtualCardProviderNetwork cardProviderNetwork)
        {
            var vcardProviderProductType = await _virtualPaymentRepo.GetVCardProviderProductTypeAsync(cardToken);

            if (vcardProviderProductType != null)
            {
                return vcardProviderProductType.ProductTypeId;
            }

            var providerProductType = await _virtualPaymentRepo.GetProviderProductTypeAsync();

            var productTypeResult = (providerProductType
                .FirstOrDefault(p => p.ProviderId == cardProviderNetwork && p.IsActive && p.IsDefault));

            if (productTypeResult?.ProductTypeId == null)
            {
                throw new NotSupportedException(
                    $"Card Product Type cannot be found for {nameof(cardToken)}: {cardToken}");
            }

            return productTypeResult.ProductTypeId;
        }

        /// <summary>
        /// Determines the <see cref="StoreProductType"/> based on the provider and store passed in.
        /// </summary>
        /// <param name="provider">The <see cref="VirtualCardProviderNetwork"/> to create the card on.</param>
        /// <param name="storeId">The store related to the lease.</param>
        /// <returns><see cref="StoreProductType"/></returns>
        private async Task<StoreProductType> ProductTypeByProviderStoreAsync(VirtualCardProviderNetwork provider, int storeId)
        {
            var storeProductType = await _virtualPaymentRepo.GetStoreProductTypeAsync(storeId);
            
            if (storeProductType != null)
            {
                return storeProductType;
            }

            // If we don't have a specific store to product mapping use the default for the provider.
            var providerProductType =
                (await _virtualPaymentRepo.GetProviderProductTypeAsync()).FirstOrDefault(p =>
                    p.ProviderId == provider && p.IsActive && p.IsDefault);

            if (providerProductType != null)
            {
                storeProductType = new StoreProductType
                {
                    StoreId = storeId,
                    ProductTypeId = providerProductType.ProductTypeId,
                    CustomerInfoRequired = false
                };
            }
            
            return storeProductType;
        }

        private async Task SetVirtualCardProductId(IEnumerable<VCard> vCards)
        {
            foreach (var vCard in vCards)
            {
                var vCardProviderProductType = await _virtualPaymentRepo.GetVCardProviderProductTypeAsync(vCard.ReferenceId);
                // If we dont have the specific type saved for the card then it was created with previous
                //  processes and all those cards were commercial products.
                vCard.ProductTypeId = vCardProviderProductType?.ProductTypeId ?? ProductType.Commercial;
            }
        }

        #endregion private helper methods
    }
}
