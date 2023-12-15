namespace VirtualPaymentService.Model.Enums
{
    /// <summary>
    /// Supported virtual card providers.
    /// </summary>
    /// <remarks>
    /// This is a subset of the values from <see cref="VPaymentProvider"/> that we support card operations (Create card, user, etc.)
    /// </remarks>
    public enum VirtualCardProviderNetwork
    {
        Marqeta = VPaymentProvider.Marqeta
    }
}
