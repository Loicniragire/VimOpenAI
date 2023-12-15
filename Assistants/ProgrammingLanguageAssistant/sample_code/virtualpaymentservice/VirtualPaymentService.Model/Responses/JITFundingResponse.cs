using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Responses
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    [ExcludeFromCodeCoverage]
    public class JITFundingResponse
    {
        /// <summary>
        /// Is the JIT funding request approved (true) or denied (false).
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// The reason for a decline of the JIT funding request. Value present only when <see cref="Approved"/> is a value of false.
        /// </summary>
        public string DeclineReason { get; set; }
    }
}
