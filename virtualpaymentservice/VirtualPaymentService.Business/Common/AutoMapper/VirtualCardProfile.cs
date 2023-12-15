using AutoMapper;
using System;
using VirtualPaymentService.Data.DynamicQueries;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.DTO.Database.Result;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Requests.Marqeta;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Responses.Marqeta;
using VirtualPaymentService.Model.Types;

namespace VirtualPaymentService.Business.Common.AutoMapper
{
    /// <summary>
    /// Mappings for virtual card contracts.
    /// </summary>
    public class VirtualCardProfile : Profile
    {
        #region Constructor
        public VirtualCardProfile()
        {
            CreateMap<MarqetaUserResponse, CardUserUpdateResponse>()
                .ForMember
                (
                    dest => dest.PhoneNumber,
                    opt => opt.MapFrom(scr => scr.Phone)
                )
                .ForMember
                (
                    dest => dest.PostalCode,
                    // In production Marqeta returns this value in zip, in sandbox value is postal_code
                    opt => opt.MapFrom(scr => string.IsNullOrEmpty(scr.Zip) ? scr.PostalCode : scr.Zip)
                );

            CreateMap<MarqetaUserResponse, CardUserResponse>()
                .ForMember
                (
                    dest => dest.PhoneNumber,
                    opt => opt.MapFrom(scr => scr.Phone)
                )
                .ForMember
                (
                    dest => dest.PostalCode,
                    // In production Marqeta returns this value in zip, in sandbox value is postal_code
                    opt => opt.MapFrom(scr => string.IsNullOrEmpty(scr.Zip) ? scr.PostalCode : scr.Zip)
                );

            CreateMap<CardUserUpdateRequest, MarqetaUserPutRequest>()
                .ForMember
                (
                    dest => dest.Phone,
                    // Marqeta requires the phone in the format +CountryCodePhoneNumber
                    opt => opt.MapFrom(scr => $"+{scr.PhoneNumberCountryCode}{scr.PhoneNumber}")
                )
                .ForMember(dest => dest.CountryCode,
                    opt => opt.MapFrom(scr => MapCountryCode(scr)))
                // Set to null for mapping, if there is a value to set the code responsible for creating/updating
                // a user at Marqeta will set this value after the mapping is done.
                .ForMember(dest => dest.AccountHolderGroupToken, opt => opt.MapFrom(scr => (string)null));

            CreateMap<MarqetaUserPostRequest, MarqetaUserPutRequest>();

            CreateMap<VirtualCardRequest, MarqetaUserMetadata>();

            CreateMap<VirtualCardRequest, MarqetaUserPostRequest>()
                .ForMember(dest => dest.Metadata, member => member.MapFrom(scr => scr))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(scr => scr.LeaseId))
                .ForMember
                (
                    dest => dest.Phone,
                    // Marqeta requires the phone in the format +CountryCodePhoneNumber
                    opt => opt.MapFrom(scr => $"+{scr.User.PhoneNumberCountryCode}{scr.User.PhoneNumber}")
                )
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(scr => scr.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(scr => scr.User.LastName))
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(scr => scr.User.Address1))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(scr => scr.User.Address2))
                .ForMember(dest => dest.City, opt => opt.MapFrom(scr => scr.User.City))
                .ForMember(dest => dest.State, opt => opt.MapFrom(scr => scr.User.State))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(scr => scr.User.PostalCode))
                .ForMember(dest => dest.CountryCode,
                    opt => opt.MapFrom(scr => MapCountryCode(scr.User)))
                // Set to null for mapping, if there is a value to set the code responsible for creating/updating
                // a user at Marqeta will set this value after the mapping is done.
                .ForMember(dest => dest.AccountHolderGroupToken, opt => opt.MapFrom(scr => (string)null));

            CreateMap<MarqetaCardVirtualCard, VCard>()
                .ForMember(dest => dest.VCardProviderId, opt => opt.MapFrom(scr => (byte)scr.CardRequest.VCardProviderId))
                .ForMember(dest => dest.LeaseId, opt => opt.MapFrom(scr => scr.CardRequest.LeaseId))
                .ForMember(dest => dest.CardBalance, opt => opt.MapFrom(scr => scr.CardRequest.CardBalance))
                .ForMember(dest => dest.AvailableBalance, opt => opt.MapFrom(scr => scr.CardRequest.AvailableBalance))
                .ForMember(dest => dest.OriginalCardBaseAmount, opt => opt.MapFrom(scr => scr.CardRequest.OriginalCardBaseAmount))
                .ForMember(dest => dest.MaxAmountGreater, opt => opt.MapFrom(scr => scr.CardRequest.MaxAmountGreater))
                .ForMember(dest => dest.MaxAmountLess, opt => opt.MapFrom(scr => scr.CardRequest.MaxAmountLess))
                .ForMember(dest => dest.ActiveToDate, opt => opt.MapFrom(scr => CalculateActiveToDate(scr.CardRequest.DaysActiveTo, scr.CardRequest.ActiveToBufferMinutes)))
                .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(scr => scr.CardCreated.CardNumber))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(scr => scr.CardCreated.ExpirationDate))
                .ForMember(dest => dest.PinNumber, opt => opt.MapFrom(scr => scr.CardCreated.CVV))
                .ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(scr => scr.CardCreated.Token))
                .ForMember(dest => dest.VCardStatusId, opt => opt.MapFrom(scr => GetCardStatusFromMarqetaCardState(scr.CardCreated.State)))
                // We are going to ignore these values for this mapping since we do not have them until
                // saved to data repo. Will query the repo for the saved card info before we respond to the caller.
                .ForMember(dest => dest.VCardId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                // Ignoring since we only set this when we create the card and not on GET requests.
                .ForMember(dest => dest.ProductTypeId, opt => opt.Ignore());

            CreateMap<VCardPurchaseAuthRequest, InsertVCardPurchaseAuthParams>()
                // Hard coded IsPush to true.
                .ForMember(dest => dest.IsPush, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.ProviderId, opt => opt.MapFrom(src => (byte)src.VCardProviderId))
                .ForMember(dest => dest.AuthDateTime, opt => opt.MapFrom(src => src.AuthorizationDateTime))
                .ForMember(dest => dest.CategoryCode, opt => opt.MapFrom(src => src.MerchantCategoryCode))
                // These are all just null when inserted.
                .ForMember(dest => dest.AvailableCredit, opt => opt.Ignore())
                .ForMember(dest => dest.CardNumber, opt => opt.Ignore())
                .ForMember(dest => dest.MerchantAcquirer, opt => opt.Ignore())
                .ForMember(dest => dest.SourceCurrencyAmount, opt => opt.Ignore())
                .ForMember(dest => dest.CurrencyConversionRate, opt => opt.Ignore())
                .ForMember(dest => dest.BillingCurrencyCode, opt => opt.Ignore());

            CreateMap<VCard, VCardUpdateParams>();
            CreateMap<VCard, VCardPurchaseAuthResponse>();

            CreateMap<VCard, VCardProviderProductType>()
                .ForMember(dest => dest.ProviderId, opt => opt.MapFrom(src => (VirtualCardProviderNetwork)src.VCardProviderId));

            CreateMap<GetVCardsRequest, GetVCardsDynamicQuery>();

            CreateMap<VCardPurchaseAuthResult, VirtualCardAuthorization>()
                .ForMember(dest => dest.MerchantCategoryCode, opt => opt.MapFrom(scr => scr.CategoryCode))
                // If Present use Prog reason since it has better detail why auth request declined during JITFunding process.
                .ForMember(dest => dest.DeclineReasonMessage, opt => opt.MapFrom(scr => string.IsNullOrEmpty(scr.ProgressiveDeclineReasonMessage) ? scr.DeclineReasonMessage : scr.ProgressiveDeclineReasonMessage));
        }

        private static string MapCountryCode(CardUserUpdateRequest scr)
        {
            var countryCode = scr.CountryCode;

            if (string.IsNullOrWhiteSpace(scr.State) && string.IsNullOrWhiteSpace(scr.CountryCode))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(scr.CountryCode) && scr.CountryCode != Constants.UnitedStatesCode)
                return countryCode;

            if (scr.State == Constants.PuertoRicoCode)
            {
                countryCode = Constants.PuertoRicoCode;
            }
            else if (scr.State == Constants.VirginIslandsCode)
            {
                countryCode = Constants.VirginIslandsCode;
            }
            else
            {
                countryCode = Constants.UnitedStatesCode;
            }

            return countryCode;
        }


        #endregion Constructor

        #region Private Methods
        /// <summary>
        /// Maps the Marqeta card state to our internal card status.
        /// </summary>
        /// <param name="cardState">Current state of the card at Marqeta.</param>
        /// <returns>Converted <see cref="CardStatus"/>.</returns>
        private static CardStatus GetCardStatusFromMarqetaCardState(MarqetaCardState cardState)
        {
            return cardState switch
            {
                MarqetaCardState.ACTIVE => CardStatus.Open,
                MarqetaCardState.TERMINATED => CardStatus.Cancelled,
                MarqetaCardState.UNACTIVATED => CardStatus.Closed,
                MarqetaCardState.SUSPENDED => CardStatus.Closed,
                _ => CardStatus.Error
            };
        }

        /// <summary>
        /// Calculates the ActiveToDate for a card.
        /// </summary>
        /// <remarks>
        /// This calculation uses the current EST date and time as the basis for the ActiveToDate. The <see cref="activeToBufferMinutes"/>
        /// are added to the current EST and then if the <see cref="daysActiveTo"/> is greater than 1 then the number of days is added.
        /// </remarks>
        /// <param name="daysActiveTo">Number of days to add to the ActiveToDate. Only values greater than 1 will extend the date.</param>
        /// <param name="activeToBufferMinutes">Number of minutes to add to the current EST time.</param>
        /// <returns></returns>
        private static DateTime CalculateActiveToDate(int daysActiveTo, int activeToBufferMinutes)
        {
            // This is where ActiveToDate of (3.5 hours) in future, plus daysActiveTo when greater than 1, occurs.
            // This was not the intended or expected behavior, based on original requirements, during inception of VPay.
            // Thus, after all consumers of vcard creation are on VPO, this logic will be refactored and improved.
            // ex. ActiveToDate observed as MST, (Now,EST) equals 2Hrs, plus activeToBufferMinutes_90min equals 1.5Hrs, totalling 3.5 Hrs.

            DateTime activeTo = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time").AddMinutes(activeToBufferMinutes);

            // This is the same logic that is used in the VPayService code. daysActiveTo values of 0 or 1 are not
            // added to the current EST+buffer datetime value.  I know this may seem like it is wrong but we currently 
            // have 8400+ stores with a value of 1 and we do not want to change the behavior for all of these stores.
            // If you want to add just one day there is not a setting that will allow this to occur.
            if (daysActiveTo > 1)
            {
                activeTo = activeTo.AddDays(daysActiveTo);
            }

            return activeTo;
        }
        #endregion Private Methods
    }
}
