namespace VirtualPaymentService.Model.Enums
{
    /// <summary>
    /// Digital wallet token status, value is mapped from the providers token status to one of these values.
    /// </summary>
    public enum DigitalWalletTokenStatus
    {
        /// <summary>
        /// This is returned when the card provider did not include a status for the wallet token.
        /// </summary>
        Unknown = 0,
        Green = 1,
        Yellow = 2,
        Red = 3
    }
}
