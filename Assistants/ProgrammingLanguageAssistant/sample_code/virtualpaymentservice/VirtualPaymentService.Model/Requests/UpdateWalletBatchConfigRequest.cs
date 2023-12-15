using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Requests
{
    [ExcludeFromCodeCoverage]
    public class UpdateWalletBatchConfigRequest
    {
        /// <summary>
        /// The last successful datetime that wallet tokens were terminated.
        /// </summary>
        [Required]
        public DateTimeOffset? LastTerminationProcessedDate { get; set; }

        /// <summary>
        /// (Optional) The user or service updating the process date.
        /// </summary>
        [MaxLength(50)]
        public string UpdatedBy { get; set; }
    }
}