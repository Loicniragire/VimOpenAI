using System.Net.Http;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.Service
{
    public class MarqetaCommercialService : MarqetaBaseService, IMarqetaCommercialService
    {
        #region Property
        protected override ProductType CardProductType => ProductType.Commercial;
        #endregion Property

        #region Constructor
        public MarqetaCommercialService(HttpClient client, AppSettings appSettings, ISecretConfigurationService secretConfigurationService)
            : base(client, appSettings, secretConfigurationService) { }
        #endregion Constructor
    }
}