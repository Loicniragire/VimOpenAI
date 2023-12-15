using System.Net.Http;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.Service
{
    public class MarqetaConsumerService : MarqetaBaseService, IMarqetaConsumerService
    {
        #region Property
        protected override ProductType CardProductType => ProductType.Consumer;
        #endregion Property

        #region Constructor
        public MarqetaConsumerService(HttpClient client, AppSettings appSettings, ISecretConfigurationService secretConfigurationService)
            : base(client, appSettings, secretConfigurationService) { }
        #endregion Constructor
    }
}