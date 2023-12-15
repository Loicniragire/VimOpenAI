using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Requests
{
    [ExcludeFromCodeCoverage]
    public class GetVCardsRequest
    {
        /// <summary>
        /// The maximum number of records to return.
        /// Min: 1.
        /// Max: 5000.
        /// Default: 100.
        /// </summary>
        [FromQuery(Name = "limit")]
        [Range(1, 5000)]
        public int Limit { get; set; } = 100;

        /// <summary>
        /// A list (0-n) of <see cref="VCard.VCardProviderId"/> to filter results on.
        /// </summary>
        [FromQuery(Name = "providerId")]
        public IList<VPaymentProvider> ProviderIds { get; set; }

        /// <summary>
        /// A list (0-n) of <see cref="VCard.VCardStatusId"/> to filter results on.
        /// </summary>
        [FromQuery(Name = "vCardStatusId")]
        public IList<CardStatus> VCardStatusIds { get; set; }

        /// <summary>
        /// A list (0-n) of <see cref="VCard.LeaseId"/> to filter results on.
        /// </summary>
        [FromQuery(Name = "leaseId")]
        public IList<long> LeaseIds { get; set; }

        /// <summary>
        /// The minimum <see cref="VCard.ExpirationDate"/> to filter results on. 
        /// Filtering will be on <see cref="VCard.ExpirationDate"/> >= the <see cref="MinExpirationDate"/>.
        /// </summary>
        [FromQuery(Name = "minExpirationDate")]
        public DateTimeOffset? MinExpirationDate { get; set; }

        /// <summary>
        /// The minimum <see cref="VCard.UpdatedDate"/> to filter results on. 
        /// Filtering will be on <see cref="VCard.UpdatedDate"/> >= the <see cref="MinUpdatedDate"/>.
        /// </summary>
        [FromQuery(Name = "minUpdatedDate")]
        public DateTimeOffset? MinUpdatedDate { get; set; }

        /// <summary>
        /// The minimum <see cref="VCard.ActiveToDate"/> to filter results on. 
        /// Filtering will be on <see cref="VCard.ActiveToDate"/> greater than or equal to the <see cref="MinActiveToDate"/>.
        /// </summary>
        [FromQuery(Name = "minActiveToDate")]
        public DateTimeOffset? MinActiveToDate { get; set; }

        /// <summary>
        /// The maximum <see cref="VCard.ExpirationDate"/> to filter results on. 
        /// Filtering will be on <see cref="VCard.ExpirationDate"/> less than or equal to the <see cref="MaxExpirationDate"/>.
        /// </summary>
        [FromQuery(Name = "maxExpirationDate")]
        public DateTimeOffset? MaxExpirationDate { get; set; }

        /// <summary>
        /// The maximum <see cref="VCard.UpdatedDate"/> to filter results on. 
        /// Filtering will be on <see cref="VCard.UpdatedDate"/> less than or equal to the <see cref="MaxUpdatedDate"/>.
        /// </summary>
        [FromQuery(Name = "maxUpdatedDate")]
        public DateTimeOffset? MaxUpdatedDate { get; set; }

        /// <summary>
        /// The maximum <see cref="VCard.ActiveToDate"/> to filter results on. 
        /// Filtering will be on <see cref="VCard.ActiveToDate"/> less than or equal to the <see cref="MaxActiveToDate"/>.
        /// </summary>
        [FromQuery(Name = "maxActiveToDate")]
        public DateTimeOffset? MaxActiveToDate { get; set; }
    }
}
