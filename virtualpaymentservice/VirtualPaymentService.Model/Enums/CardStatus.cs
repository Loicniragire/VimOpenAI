namespace VirtualPaymentService.Model.Enums
{
    /// <summary>
    /// The current internal Progressive status of the virtual card. 
    /// </summary>
    public enum CardStatus
    {
        Null = 0,
        Open = 1,
        Closed = 2,
        Authorized = 3,
        Posted = 4,
        Cancelled = 5,
        None = 6,
        InvoiceMismatch = 7,
        Error = 8
    }
}
