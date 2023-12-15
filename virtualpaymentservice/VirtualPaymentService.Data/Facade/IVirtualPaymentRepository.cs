using VirtualPaymentService.Data.Interface;

namespace VirtualPaymentService.Data.Facade
{
    public interface IVirtualPaymentRepository : IWalletBatchConfigRepository, IVCardRepository, IProviderRepository, IVCardProviderProductTypeRepository
    { }
}