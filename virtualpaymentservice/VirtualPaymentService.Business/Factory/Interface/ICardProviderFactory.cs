using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.Factory.Interface
{
    public interface ICardProviderFactory
    {
        public IDigitalWalletProvider GetWalletProvider(VPaymentProvider provider);

        /// <summary>
        /// Gets the card provider based on the <see cref="VirtualCardProviderNetwork"/> passed in.
        /// </summary>
        /// <param name="provider">The <see cref="VirtualCardProviderNetwork"/></param>
        /// <returns><see cref="ICardProvider"/></returns>
        public ICardProvider GetCardProvider(VirtualCardProviderNetwork provider);
    }
}
