namespace VirtualPaymentService.Model.Enums
{
    /// <summary>
    /// The wallet status values provided by Marqeta.
    /// </summary>
    /// <remarks>
    /// https://www.marqeta.com/docs/developer-guides/managing-the-lifecycle-of-digital-wallet-tokens#_transitioning_token_states
    /// </remarks>
    public enum MarqetaDigitalWalletTokenStatus
    {
        /// <summary>
        /// Zero is set when Marqeta does not send a token status in the response. 
        /// This is not one of the values Marqeta will send us.
        /// </summary>
        /// <remarks>
        /// The state property in the Marqeta documentation notes this is conditionally returned.
        /// </remarks>
        UNKNOWN = 0,
        REQUESTED = 1,
        REQUEST_DECLINED = 2,
        ACTIVE = 3,
        SUSPENDED = 4,
        TERMINATED = 5
    }
}
