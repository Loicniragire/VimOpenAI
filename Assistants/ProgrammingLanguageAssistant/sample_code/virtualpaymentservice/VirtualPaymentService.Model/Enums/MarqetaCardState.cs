namespace VirtualPaymentService.Model.Enums
{
    /// <summary>
    /// The state of the card on the Marqeta network.
    /// </summary>
    public enum MarqetaCardState
    {
        ACTIVE = 1,
        SUSPENDED = 2,
        TERMINATED = 3,
        UNSUPPORTED = 4,
        UNACTIVATED = 5,
        LIMITED = 6
    }
}
