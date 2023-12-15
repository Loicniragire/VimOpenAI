using Microsoft.Extensions.DependencyInjection;
using System;
using VirtualPaymentService.Business.Factory.Interface;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Provider;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.Factory
{
    public class CardProviderFactory : ICardProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CardProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDigitalWalletProvider GetWalletProvider(VPaymentProvider provider)
        {
            return provider switch
            {
                VPaymentProvider.Marqeta => (IDigitalWalletProvider)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(MarqetaProvider)),
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, $"VPaymentProvider of {provider} is not supported.")
            };
        }

        /// <inheritdoc/>
        public ICardProvider GetCardProvider(VirtualCardProviderNetwork provider)
        {
            return provider switch
            {
                VirtualCardProviderNetwork.Marqeta => (ICardProvider)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(MarqetaProvider)),
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, $"{nameof(VirtualCardProviderNetwork)} of {provider} is not supported.")
            };
        }

    }
}
