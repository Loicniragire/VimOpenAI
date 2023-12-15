using System;
using System.Collections.Generic;
using System.Linq;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Data.DynamicQueries
{
#pragma warning disable S2376 // Allow us to take advantage of AutoMapper.
    public class GetVCardsDynamicQuery : DynamicQueryBuilderBase
    {
        /// <inheritdoc cref="GetVCardsRequest.Limit" />
        public int Limit { get; set; }

        /// <inheritdoc cref="GetVCardsRequest.ProviderIds" />
        public IList<VPaymentProvider> ProviderIds
        {
            set
            {
                if (!value.Any())
                {
                    return;
                }

                AddParameterizedCondition(
                    nameof(VCard.VCardProviderId),
                    ConditionalOperator.IN,
                    value);
            }
        }

        /// <inheritdoc cref="GetVCardsRequest.VCardStatusIds" />
        public IList<CardStatus> VCardStatusIds
        {
            set
            {
                if (!value.Any())
                {
                    return;
                }

                AddParameterizedCondition(
                    nameof(VCard.VCardStatusId),
                    ConditionalOperator.IN,
                    value);
            }
        }

        /// <inheritdoc cref="GetVCardsRequest.LeaseIds" />
        public IList<long> LeaseIds
        {
            set
            {
                if (!value.Any())
                {
                    return;
                }

                AddParameterizedCondition(
                    nameof(VCard.LeaseId),
                    ConditionalOperator.IN,
                    value);
            }
        }

        /// <inheritdoc cref="GetVCardsRequest.MinExpirationDate" />
        public DateTimeOffset? MinExpirationDate
        {
            set
            {
                if (value is null)
                {
                    return;
                }

                AddParameterizedCondition(
                    nameof(VCard.ExpirationDate),
                    ConditionalOperator.GTEQ,
                    // ExpirationDate is stored as UTC
                    value.Value.UtcDateTime);
            }
        }

        /// <inheritdoc cref="GetVCardsRequest.MinUpdatedDate" />
        public DateTimeOffset? MinUpdatedDate
        {
            set
            {
                if (value is null)
                {
                    return;
                }

                AddParameterizedCondition(
                    nameof(VCard.UpdatedDate),
                    ConditionalOperator.GTEQ,
                    OffsetToMst((DateTimeOffset)value));
            }
        }

        /// <inheritdoc cref="GetVCardsRequest.MinActiveToDate" />
        public DateTimeOffset? MinActiveToDate
        {
            set
            {
                if (value is null)
                {
                    return;
                }

                AddParameterizedCondition(
                    nameof(VCard.ActiveToDate),
                    ConditionalOperator.GTEQ,
                    OffsetToMst((DateTimeOffset)value));
            }
        }

        /// <inheritdoc cref="GetVCardsRequest.MaxExpirationDate" />
        public DateTimeOffset? MaxExpirationDate
        {
            set
            {
                if (value is null)
                {
                    return;
                }

                AddParameterizedCondition(
                    nameof(VCard.ExpirationDate),
                    ConditionalOperator.LTEQ,
                    // ExpirationDate is stored as UTC
                    value.Value.UtcDateTime,
                    nameof(MaxExpirationDate));
            }
        }

        /// <inheritdoc cref="GetVCardsRequest.MaxUpdatedDate" />
        public DateTimeOffset? MaxUpdatedDate
        {
            set
            {
                if (value is null)
                {
                    return;
                }

                AddParameterizedCondition(
                    nameof(VCard.UpdatedDate),
                    ConditionalOperator.LTEQ,
                    OffsetToMst((DateTimeOffset)value),
                    nameof(MaxUpdatedDate));
            }
        }

        /// <inheritdoc cref="GetVCardsRequest.MaxActiveToDate" />
        public DateTimeOffset? MaxActiveToDate
        {
            set
            {
                if (value is null)
                {
                    return;
                }

                AddParameterizedCondition(
                    nameof(VCard.ActiveToDate),
                    ConditionalOperator.LTEQ,
                    OffsetToMst((DateTimeOffset)value),
                    nameof(MaxActiveToDate));
            }
        }

        /// <summary>
        /// Converts a given <see cref="DateTimeOffset"/> to an MST <see cref="DateTime"/>
        /// </summary>
        /// <param name="dateOffset"></param>
        /// <returns></returns>
        private static DateTime OffsetToMst(DateTimeOffset dateOffset)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateOffset, "Mountain Standard Time").DateTime;
        }
    }
#pragma warning restore S2376 // Allow us to take advantage of AutoMapper.
}
