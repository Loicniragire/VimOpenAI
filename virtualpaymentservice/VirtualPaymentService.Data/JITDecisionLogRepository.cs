using Dapper;
using ProgLeasing.System.Data.Contract;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VirtualPaymentService.Data.Configuration;
using VirtualPaymentService.Model.Domain;

namespace VirtualPaymentService.Data
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    [ExcludeFromCodeCoverage]
    public class JITDecisionLogRepository : Repository<JITDecisionLog>, IJITDecisionLogRepository
    {
        public JITDecisionLogRepository(IDbConnectionFactory dbConnectionFactory)
            : base(Constants.VirtualPaymentDatabase, dbConnectionFactory) { }

        /// <inheritdoc/>
        public async Task SaveJITDecisionLogAsync(JITDecisionLog decision)
        {
            var procedure = "VirtualPayment.VCard.JustInTimeDecisionLog_Save";
            var sqlParams = new
            {
                LeaseId = decision.LeaseId,
                ReferenceId = decision.VCardReferenceId,
                TransactionToken = decision.TransactionToken,
                TransactionAmount = decision.TransactionAmount,
                TransactionDate = decision.TransactionDate,
                TransactionType = decision.TransactionType,
                TransactionState = decision.TransactionState,
                Approved = decision.Approved,
                DeclineReasonId = (int)decision.DeclineReason,
                DecisionDate = decision.DecisionDate,
                LeaseStatus = decision.LeaseStatus,
                IsMinAmountRequired = decision.IsMinAmountRequired,
                UseStateValidation = decision.UseStateValidation,
                StoreAddressState = decision.StoreAddressState,
                AvailableBalance = decision.AvailableBalance,
                CardActiveToDate = decision.CardActiveToDate,
                CardStatusId = (int)decision.CardStatus
            };

            using var connection = GetConnection;

            await connection.ExecuteAsync(procedure, sqlParams, commandType: CommandType.StoredProcedure);
        }
    }
}
