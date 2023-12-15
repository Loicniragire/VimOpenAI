namespace VirtualPaymentService.Model.Types
{
    public static class Constants
    {
        public static class Authorizations
        {
            public const string ReversalType = "0400";
            public const string AuthorizationType = "0100";

            public const string Approval = "Approval";
            public const string Decline = "Decline";

            public const string DbDuplicateAuthMessage = "VCardPurchaseAuth_ip: attempted to insert duplicate record for ProviderAuthorizationId";
            public const string DbMissingVCardMessage = "No matching card was found for given provider and card number/reference ID.";
        }
        
        public const string UnitedStatesCode = "US";
        public const string PuertoRicoCode = "PR";
        public const string VirginIslandsCode = "VI";
    }
}
