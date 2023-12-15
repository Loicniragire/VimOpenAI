using ProgLeasing.System.Logging.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.JIT;
using VirtualPaymentService.Business.JIT.Rules;
using VirtualPaymentService.Data;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Provider
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    public class JITDecisionProvider : IJITDecisionProvider
    {
        private readonly ILogger<IJITDecisionProvider> _logger;
        private readonly IJITDecisionLogRepository _jitLogRepository;
        private readonly IVCardRepository _repository;

        public JITDecisionProvider
        (
            ILogger<IJITDecisionProvider> logger,
            IJITDecisionLogRepository jitLogRepository,
            IVCardRepository repository
        )
        {
            _logger = logger;
            _jitLogRepository = jitLogRepository;
            _repository = repository;
        }

        public async Task<JITFundingResponse> ProcessJITDecisionAsync(JITFundingRequest request)
        {
            try
            {
                _logger.Log(LogLevel.Info, $"JITFundingRequest received: {JsonSerializer.Serialize(request)}");

                var rules = new List<IJITFundingRule>()
                {
                    new TransactionTypeRule(),
                    new ActiveToDateRule(),
                    new CardStatusRule(),
                    new AvailableBalanceRule(),
                    new FundedRule(),
                    new StateRule()
                };

                var vCard = await _repository.GetVCardAsync((long)request.LeaseId, request.ProviderCardId);

                if (vCard == null)
                {
                    throw new HttpRequestException($"Failed to locate a virtual card for LeaseId {request.LeaseId} and ProviderCardId {request.ProviderCardId}.", null, HttpStatusCode.NotFound);
                }

                var decision = new JITDecision();
                foreach (var rule in rules)
                {
                    decision = rule.Execute(request, vCard);
                    if (!decision.Approved)
                        break;
                }

                var response = new JITFundingResponse
                {
                    Approved = decision.Approved,
                    DeclineReason = DeclineReasons.GetText(decision.DeclineReason)
                };

                _logger.Log(LogLevel.Info, decision.Approved
                        ? $"Transaction for {request.LeaseId} was approved"
                        : $"Transaction for {request.LeaseId} was declined, Decline Reason: {response.DeclineReason}",
                    null, new Dictionary<string, object>
                        { { nameof(request.LeaseId), request.LeaseId } });

                // Persist the decision to the log.
                // NOTE: A failure when persisting to the log in the DB does not cause a failure to return to the caller.
                _ = SaveJITDecisionLogAsync(request, vCard, decision);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"An error occurred during JIT funding: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Persists the JIT decision to the data store.
        /// </summary>
        /// <remarks>
        /// A failure when persisting to the log in the DB does not cause a failure to return to the caller.
        /// Currently the log table is just being used to manually evaluate leases when issues are encountered.
        /// </remarks>
        /// <param name="request">The <see cref="JITFundingRequest"/> passed into the endpoint from the caller.</param>
        /// <param name="vCard">The <see cref="VCard"/> information located from the data store.</param>
        /// <param name="decision">The <see cref="JITDecision"/> being returned top the caller.</param>
        private async Task SaveJITDecisionLogAsync(JITFundingRequest request, VCard vCard, JITDecision decision)
        {
            var decisionLog = new JITDecisionLog
            {
                LeaseId = (long)request.LeaseId,
                VCardReferenceId = request.ProviderCardId,
                TransactionToken = request.TransactionToken,
                TransactionAmount = request.TransactionAmount,
                TransactionDate = request.TransactionDate,
                TransactionType = request.TransactionType,
                TransactionState = request.TransactionState,
                LeaseStatus = request.LeaseStatus,
                IsMinAmountRequired = request.IsMinAmountRequired,
                UseStateValidation = request.UseStateValidation,
                StoreAddressState = request.StoreAddressState,
                AvailableBalance = vCard.AvailableBalance,
                CardActiveToDate = vCard.ActiveToDate,
                CardStatus = vCard.VCardStatusId,
                Approved = decision.Approved,
                DeclineReason = decision.DeclineReason,
                DecisionDate = DateTime.UtcNow
            };

            try
            {
                await _jitLogRepository.SaveJITDecisionLogAsync(decisionLog);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error saving JITDecisionLog: {JsonSerializer.Serialize(decisionLog)}", ex);
            }
        }
    }
}
