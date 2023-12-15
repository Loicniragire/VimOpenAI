using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using ProgLeasing.System.Data.Contract;
using ProgLeasing.System.Logging.Contract;

namespace VirtualPaymentService.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class TransactionScopeUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ILogger _logger;

        public TransactionScopeUnitOfWorkFactory(ILogger<TransactionScopeUnitOfWorkFactory> logger)
        {
            _logger = logger;
        }

        public IUnitOfWork Begin()
        {
            return Begin(IsolationLevel.ReadCommitted);
        }

        public IUnitOfWork Begin(IsolationLevel isolationLevel)
        {
            var txId = Guid.NewGuid();
            _logger.Log(LogLevel.Debug, "Setting up a new TransactionScope identified by: {TransactionId}; with isolation level: {IsolationLevel}", metadata: new Dictionary<string, object> { { "TransactionId", txId.ToString() }, { "IsolationLevel", isolationLevel.ToString() } });
            return new TransactionScopeUnitOfWork(isolationLevel, txId);
        }

    }
}
