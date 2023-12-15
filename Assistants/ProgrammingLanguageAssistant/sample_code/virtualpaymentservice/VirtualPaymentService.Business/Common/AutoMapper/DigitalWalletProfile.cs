using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Requests.Marqeta;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Responses.Marqeta;

namespace VirtualPaymentService.Business.Common.AutoMapper
{
    public class DigitalWalletProfile : Profile
    {
        // Mappings for all future Digital Wallet provision requests should go here.
        public DigitalWalletProfile()
        {
            CreateMap<ApplePayProvisioningData, MarqetaProvisionApplePayRequest>()
                .ForMember
                (
                    dest => dest.Certificates,
                    opt => opt.MapFrom
                    (
                        src => new string[2]
                        {
                            src.LeafCertificate, // Marqeta requires the LeafCertificate to be first
                            src.SubCACertificate
                        }
                    )
                );

            CreateMap<MarqetaProvisionApplePayResponse, ApplePayTokenizationResponse>();
            CreateMap<GooglePayProvisioningData, MarqetaProvisionGooglePayRequest>();
            CreateMap<MarqetaProvisionGooglePayResponse, GooglePayTokenizationResponse>();
            CreateMap<MarqetaProvisionGooglePayTokenizationData, GooglePayTokenizationData>();
            CreateMap<MarqetaProvisionGooglePayUserAddress, GooglePayUserAddress>()
                .ForMember
                (
                    dest => dest.PostalCode,
                    opt => opt.MapFrom(src => src.PostalCode ?? src.ZipCode)
                );

            CreateMap<MarqetaDigitalWalletTokenStatus, DigitalWalletTokenStatus>()
                .ConvertUsingEnumMapping
                (
                    opt => opt
                        // Maps the value sent by Marqeta to our internal status
                        .MapValue(MarqetaDigitalWalletTokenStatus.REQUESTED, DigitalWalletTokenStatus.Green)
                        .MapValue(MarqetaDigitalWalletTokenStatus.ACTIVE, DigitalWalletTokenStatus.Green)
                        .MapValue(MarqetaDigitalWalletTokenStatus.SUSPENDED, DigitalWalletTokenStatus.Yellow)
                        .MapValue(MarqetaDigitalWalletTokenStatus.REQUEST_DECLINED, DigitalWalletTokenStatus.Red)
                        .MapValue(MarqetaDigitalWalletTokenStatus.TERMINATED, DigitalWalletTokenStatus.Red)
                );

            CreateMap<DigitalWalletTokenStatus, MarqetaDigitalWalletTokenStatus>()
                .ConvertUsingEnumMapping
                (
                    opt => opt
                        // Maps the value sent by Prog internal status to Marqeta status
                        .MapValue(DigitalWalletTokenStatus.Green, MarqetaDigitalWalletTokenStatus.ACTIVE)
                        .MapValue(DigitalWalletTokenStatus.Yellow, MarqetaDigitalWalletTokenStatus.SUSPENDED)
                        .MapValue(DigitalWalletTokenStatus.Red, MarqetaDigitalWalletTokenStatus.TERMINATED)
                );

            CreateMap<MarqetaDigitalWalletToken, DigitalWalletToken>()
                .ForMember
                (
                    dest => dest.WalletToken,
                    opt => opt.MapFrom(src => src.Token)
                )
                .ForMember
                (
                    dest => dest.DeviceType,
                    opt => opt.MapFrom(src => src.Device.Type)
                )
                .ForMember
                (
                    dest => dest.WalletTokenStatus,
                    opt => opt.MapFrom(src => GetDigitalWalletTokenStatus(src.State, src.FulfillmentStatus))
                );

            CreateMap<MarqetaDigitalWalletTokensForCardResponse, DigitalWalletTokenResponse>()
                .ForMember
                (
                    dest => dest.DigitalWalletTokens,
                    opt => opt.MapFrom(src => src.Data)
                );

            CreateMap<DigitalWalletTokenTransitionRequest, MarqetaWalletTokenTransitionRequest>()
                .ForPath
                (
                    dest => dest.DigitalWalletToken.Token,
                    opt => opt.MapFrom(src => src.DigitalWalletToken.WalletToken)
                )
                .ForMember
                (
                    dest => dest.ReasonCode,
                    opt => opt.MapFrom(src => src.WalletTransitionReasonCode)
                )
                .ForMember
                (
                    dest => dest.State,
                    opt => opt.MapFrom(src => src.WalletTokenTransitionStatus)
                );

            CreateMap<MarqetaWalletTokenTransitionResponse, DigitalWalletTokenTransitionResponse>()
                .ForMember
                (
                    dest => dest.WalletTransitionState,
                    opt => opt.MapFrom(src => src.State)
                );

            CreateMap<WalletBatchConfig, WalletBatchConfigResponse>();
        }

        /// <summary>
        /// Maps the <see cref="DigitalWalletTokenStatus"/> from the source <see cref="MarqetaDigitalWalletTokenStatus"/>.
        /// When a token status is in a requested state the fulfillment status must be used to determine the mapped status.
        /// </summary>
        /// <param name="sourceTokenState">State of token from Marqeta.</param>
        /// <param name="sourceFulfillmentStatus">The fulfillment status from Marqeta.</param>
        /// <returns>The <see cref="DigitalWalletTokenStatus"/> mapped token status value to use.</returns>
        private static DigitalWalletTokenStatus GetDigitalWalletTokenStatus(MarqetaDigitalWalletTokenStatus sourceTokenState, string sourceFulfillmentStatus)
        {
            return sourceTokenState switch
            {
                MarqetaDigitalWalletTokenStatus.ACTIVE => DigitalWalletTokenStatus.Green,
                MarqetaDigitalWalletTokenStatus.SUSPENDED => DigitalWalletTokenStatus.Yellow,
                MarqetaDigitalWalletTokenStatus.TERMINATED => DigitalWalletTokenStatus.Red,
                MarqetaDigitalWalletTokenStatus.REQUEST_DECLINED => DigitalWalletTokenStatus.Red,
                MarqetaDigitalWalletTokenStatus.REQUESTED when sourceFulfillmentStatus == "DECISION_GREEN" => DigitalWalletTokenStatus.Green,
                // Not sure if we will have this combination but here in case we do
                MarqetaDigitalWalletTokenStatus.REQUESTED when sourceFulfillmentStatus == "PROVISIONED" => DigitalWalletTokenStatus.Green,
                MarqetaDigitalWalletTokenStatus.REQUESTED when sourceFulfillmentStatus == "DECISION_YELLOW" => DigitalWalletTokenStatus.Yellow,
                MarqetaDigitalWalletTokenStatus.REQUESTED when sourceFulfillmentStatus == "DECISION_RED" => DigitalWalletTokenStatus.Red,
                MarqetaDigitalWalletTokenStatus.REQUESTED when sourceFulfillmentStatus == "REJECTED" => DigitalWalletTokenStatus.Red,
                _ => DigitalWalletTokenStatus.Unknown
            };
        }
    }
}