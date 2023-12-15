using AutoMapper;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Data.Facade;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Types;

namespace VirtualPaymentService.Business.Provider
{
    public class VPayTransactionProvider : IVPayTransactionProvider
    {
        private readonly ILogger<IVPayTransactionProvider> _logger;
        private readonly IMapper _mapper;
        private readonly IVPayTransactionRepository _vPayTrxRepo;

        public VPayTransactionProvider(
            ILogger<IVPayTransactionProvider> logger,
            IMapper mapper,
            IVPayTransactionRepository vPayTrxRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _vPayTrxRepo = vPayTrxRepo;
        }


        /// <inheritdoc />
        public async Task<AddVCardAuthorizationResult> AddVCardAuthorizationAsync(VCardPurchaseAuthRequest request)
        {
            try
            {
                var mappedAuth = _mapper.Map<InsertVCardPurchaseAuthParams>(request);
                var insertAuthResult = await _vPayTrxRepo.InsertVCardPurchaseAuthAsync(mappedAuth);
                var vCard = await _vPayTrxRepo.GetVCardByIdAsync(insertAuthResult.VCardId);

                if (mappedAuth.Response == Constants.Authorizations.Approval)
                {
                    mappedAuth.Amount = request.IsReversal ? mappedAuth.Amount * -1 : mappedAuth.Amount;
                    vCard.AvailableBalance -= mappedAuth.Amount;
                    vCard.CardBalance += mappedAuth.Amount;

                    // Don't change the VCardStatus if it is cancelled.
                    if (vCard.VCardStatusId != CardStatus.Cancelled)
                    {
                        vCard.VCardStatusId = vCard.CardBalance <= 0 ? CardStatus.Open : CardStatus.Authorized;
                    }

                    _ = await _vPayTrxRepo.UpdateVCardAsync(_mapper.Map<VCardUpdateParams>(vCard));
                }

                return new AddVCardAuthorizationResult(_mapper.Map<VCardPurchaseAuthResponse>(vCard), false);
            }
            catch (DbException ex)
            {
                if (ex.Message == Constants.Authorizations.DbMissingVCardMessage
                    || ex.Message.Contains(Constants.Authorizations.DbDuplicateAuthMessage))
                {
                    _logger.Log(LogLevel.Warn, $"Error inserting purchase auth: {ex.Message}", ex);

                    return new AddVCardAuthorizationResult { SoftFail = true };
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task<GetVirtualCardAuthorizationsResponse> GetVirtualCardAuthorizationsAsync(int vCardId)
        {
            var authorizations = await _vPayTrxRepo.GetVCardPurchaseAuthAsync(vCardId);
            var response = new GetVirtualCardAuthorizationsResponse();

            foreach (var authorization in authorizations)
            {
                response.Authorizations.Add(_mapper.Map<VirtualCardAuthorization>(authorization));
            }

            return response;
        }

        /// <inheritdoc />
        public async Task AddSettlementTransactionAsync(SettlementTransactionRequest request)
        {
            var failedProviderTransactionIdentifiers = new List<string>();
            var addTransactionExceptions = new List<Exception>();

            // Only inserting one transaction at a time so one failure 
            // does not cause all requested transactions to fail.
            foreach (SettlementTransaction transaction in request.SettlementTransactions)
            {
                try
                {
                    await _vPayTrxRepo.InsertSettlementTransactionAsync(transaction);
                }
                catch (Exception ex)
                {
                    // If we get a duplicate error then we just report back success. No need to have
                    // the caller have to identify a duplicate and not re-submit it. Will just allow
                    // the process to log that we received a duplicate so we can track how often this occurs.
                    if (ex.Message.Contains("Cannot insert duplicate key row in object \'Store.VPayReconciliation\' with unique index \'UX_VPayReconciliation_ProviderTransactionIdentifier"))
                    {
                        _logger.Log(
                            LogLevel.Warn,
                            $"Duplicate insert request for {nameof(transaction.ProviderTransactionIdentifier)} {transaction.ProviderTransactionIdentifier} and {nameof(transaction.LeaseId)} {transaction.LeaseId}. No resolution is required, just a warning and to help document a duplicate insert was requested, service reported back a success response.",
                            exception: ex,
                            metadata: new Dictionary<string, object>
                            {
                                {nameof(SettlementTransaction), JsonSerializer.Serialize(transaction)}
                            });
                    }
                    // If the error is something else then we need to keep track of the transaction that failed so 
                    // we can report back to the caller what rows failed to get inserted from the batch they submitted.
                    else
                    {
                        // Keep track of failed transaction identifier so it can be returned to caller.
                        failedProviderTransactionIdentifiers.Add(transaction.ProviderTransactionIdentifier);

                        addTransactionExceptions.Add(ex);

                        _logger.Log(
                            LogLevel.Error,
                            $"Failed to insert settlement transaction {nameof(transaction.ProviderTransactionIdentifier)} {transaction.ProviderTransactionIdentifier} for {nameof(transaction.LeaseId)} {transaction.LeaseId}.",
                            exception: ex,
                            metadata: new Dictionary<string, object>
                            {
                                {nameof(SettlementTransaction), JsonSerializer.Serialize(transaction)}
                            });
                    }
                }
            }

            if (failedProviderTransactionIdentifiers.Count > 0)
            {
                // Message will be returned in endpoint response.
                throw new AggregateException($"{failedProviderTransactionIdentifiers.Count} of the {request.SettlementTransactions.Count} {nameof(request.SettlementTransactions)} failed to be created. The list of {nameof(SettlementTransaction.ProviderTransactionIdentifier)} values that failed are: ({String.Join(",", failedProviderTransactionIdentifiers)}) ~",
                                             addTransactionExceptions);
            }
        }

        /// <inheritdoc />
        public async Task<GetSettlementTransactionsResponse> GetSettlementTransactionsAsync(int leaseId)
        {
            var settlements = await _vPayTrxRepo.GetSettlementTransactionAsync(leaseId);

            var response = new GetSettlementTransactionsResponse();
            response.Settlements = settlements;

            return response;
        }
    }
}
