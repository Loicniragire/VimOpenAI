using System;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using ProgLeasing.System.Data.Contract;

namespace VirtualPaymentService.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class TransactionScopeUnitOfWork : IUnitOfWork
    {
        private bool _disposed = false;

        private readonly TransactionScope _transactionScope;

        /// <summary>
        /// Used mostly for logging to identify the transaction.
        /// </summary>
        public Guid TransactionId { get; }

        public TransactionScopeUnitOfWork(IsolationLevel isolationLevel, Guid? transactionId = null)
        {
            _transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = isolationLevel,
                    Timeout = TimeSpan.FromSeconds(30)
                }, TransactionScopeAsyncFlowOption.Enabled);

            TransactionId = transactionId ?? Guid.NewGuid();
        }

        public void Commit()
        {
            _transactionScope.Complete();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transactionScope.Dispose();
                }
                _disposed = true;
            }
        }

        public void Rollback()
        {
            _transactionScope.Dispose();
        }
    }
}
