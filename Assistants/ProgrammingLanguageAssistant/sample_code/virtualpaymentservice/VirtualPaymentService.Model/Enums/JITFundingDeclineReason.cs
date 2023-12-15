using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Enums
{
    [SuppressMessage("Minor Code Smell", "S2342:Enumeration types should comply with a naming convention", Justification = "Reviewed")]
    public enum JITFundingDeclineReason
    {
        None = 0,
        CardExpired = 1,
        AmountMismatch = 2,
        AmountTooHigh = 3,
        InvalidCardStatus = 4,
        InvalidTransactionType = 5,
        LeaseAlreadyFunded = 6,
        StateMismatch = 7,
        StateMissing = 8,
        PreviouslyAuthorized = 9
    }
}
