using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.Configuration
{
    /// <summary>
    /// Required settings located in appsettings.json for virtual card provider.  
    /// There is a setting entry for each <see cref="CardProvider"/> and <see cref="ProductType"/> combination.
    /// </summary>
    public class CardProviderSetting
    {
        /// <summary>
        /// The card provider
        /// </summary>
        public VirtualCardProviderNetwork CardProvider { get; set; }

        /// <summary>
        /// The card product the provider supports.
        /// </summary>
        public ProductType ProductType { get; set; }

        /// <summary>
        /// The base URL to use when calling the card provider APIs.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Should the card creation process set an initial (random) pin.
        /// </summary>
        public bool SetInitalPin { get; set; }

        /// <summary>
        /// The product token to pass to the card provider when creating a new card.
        /// </summary>
        /// <remarks>
        /// For Marqeta you can call the GET /v3/cardproducts endpoint at Marqeta to see the card product tokens we have setup.
        /// </remarks>
        public string VirtualCardProductToken { get; set; }

        /// <summary>
        /// The user key name to locate the value from the secrets store.
        /// </summary>
        public string ApiUserKeyName { get; set; }

        /// <summary>
        /// The password key name to locate the value from the secrets store.
        /// </summary>
        public string ApiPasswordKeyName { get; set; }

        /// <summary>
        /// The token to send when creating/updating users on Marqeta network.
        /// </summary>
        /// <remarks>
        /// If set will be added to the POST/PUT user request for Marqeta cards.
        /// </remarks>
        public string MarqetaAccountHolderGroupToken { get; set; } = null;
    }
}
