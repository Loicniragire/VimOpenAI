using System;

namespace VirtualPaymentService.Model.Domain
{
    public class WalletBatchConfig
    {
        /// <summary>
        /// The record id and primary key within the WalletBatchConfig table.
        /// </summary>
        public int WalletBatchConfigId { get; set; }

        /// <summary>
        /// A <see cref="DateTimeOffset"/> reflecting the last successful wallet token batch termination process.
        /// </summary>
        public DateTimeOffset LastTerminationProcessedDate { get; set; }
    }
}
