using System;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests.Marqeta;
using VirtualPaymentService.Model.Responses.Marqeta;

namespace VirtualPaymentService.Business.Mocks
{
    /// <summary>
    /// Mocks responses for Marqeta mobile wallet endpoints that do not 
    /// function in their test/sandbox environment. 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class MarqetaMobileWalletMocks
    {

        /// <summary>
        /// Generates a successful mock response matching the Marqeta endpoint POST /digitalwallettokentransitions
        /// https://www.marqeta.com/docs/core-api/digital-wallets-management#_create_digital_wallet_token_transition
        /// </summary>
        public static MarqetaWalletTokenTransitionResponse PostDigitalWalletTokenTransitionSuccessMockResponse(MarqetaWalletTokenTransitionRequest request)
        {
            var mockedReponse = new MarqetaWalletTokenTransitionResponse()
            {
                // Generate random value for token value.
                WalletTransitionToken = Guid.NewGuid().ToString(),
                // Return state the request wanted the token to change to.
                State = request.State,
                // Not a value Marqeta would return, included to help note this is a mock.
                FulfillmentStatus = "state.mock.response",
                // Return value based on State that was passed in request
                WalletTransitionType =
                    request.State switch
                    {
                        MarqetaDigitalWalletTokenStatus.TERMINATED => "DECISION_RED",
                        MarqetaDigitalWalletTokenStatus.SUSPENDED => "DECISION_YELLOW",
                        _ => "DECISION_GREEN"
                    }
            };

            return mockedReponse;
        }

        /// <summary>
        /// Generates a successful mock response matching the Marqeta endpoint GET /digitalwallettokens/card/{card_token}
        /// https://www.marqeta.com/docs/core-api/digital-wallets-management#_list_digital_wallet_tokens_for_card
        /// </summary>
        public static MarqetaDigitalWalletTokensForCardResponse GetDigitalWalletTokensByCardTokenSuccessMockResponse(string cardToken)
        {
            var mockedReponse = new MarqetaDigitalWalletTokensForCardResponse()
            {
                // Return 3 tokens Red, Yellow and Green.
                Count = 3,
                Data = new System.Collections.Generic.List<MarqetaDigitalWalletToken>
                {
                    // Green token
                    new MarqetaDigitalWalletToken
                    {
                        Token = Guid.NewGuid().ToString(),
                        CardToken = cardToken,
                        State = MarqetaDigitalWalletTokenStatus.ACTIVE,
                        StateReason = "Approved via automated validation",
                        FulfillmentStatus = "DECISION_GREEN",
                        Device = new MarqetaDigitalWalletTokenDevice
                        {
                            Type = "MOBILE_PHONE"
                        }
                    },
                    // Yellow token
                    new MarqetaDigitalWalletToken
                    {
                        Token = Guid.NewGuid().ToString(),
                        CardToken = cardToken,
                        State = MarqetaDigitalWalletTokenStatus.REQUESTED,
                        StateReason = "Yellow path requiring SMS or call to Prog support.",
                        FulfillmentStatus = "DECISION_YELLOW",
                        Device = new MarqetaDigitalWalletTokenDevice
                        {
                            Type = "MOBILE_PHONE"
                        }
                    },
                    // Red token
                    new MarqetaDigitalWalletToken
                    {
                        Token = Guid.NewGuid().ToString(),
                        CardToken = cardToken,
                        State = MarqetaDigitalWalletTokenStatus.TERMINATED,
                        StateReason = "Token was rejected and set to terminated",
                        FulfillmentStatus = "DECISION_RED",
                        Device = new MarqetaDigitalWalletTokenDevice
                        {
                            Type = "MOBILE_PHONE"
                        }
                    }
                }
            };

            return mockedReponse;
        }

        /// <summary>
        /// Generates a successful mock response matching the Marqeta endpoint POST /digitalwalletprovisionrequests/applepay
        /// https://www.marqeta.com/docs/core-api/digital-wallets-management#_create_digital_wallet_token_provision_request_for_apple_pay
        /// </summary>
        public static MarqetaProvisionApplePayResponse PostProvisionApplePaySuccessMockResponse(MarqetaProvisionApplePayRequest request)
        {
            var mockedResponse = new MarqetaProvisionApplePayResponse()
            {
                CreatedTime = DateTime.Now,
                LastModifiedTime = DateTime.Now,
                // Return the card token sent in the request.
                CardToken = request.CardToken,
                EncryptedPassData = $"{Guid.NewGuid()}+{Guid.NewGuid()}",
                ActivationData = $"{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}",
                EphemeralPublicKey = $"{Guid.NewGuid().ToString("N")}/{Guid.NewGuid().ToString("N")}"
            };

            return mockedResponse;
        }

        /// <summary>
        /// Generates a successful mock response matching the Marqeta endpoint POST /digitalwalletprovisionrequests/androidpay
        /// https://www.marqeta.com/docs/core-api/digital-wallets-management#_create_digital_wallet_token_provision_request_for_google_pay
        /// </summary>
        public static MarqetaProvisionGooglePayResponse PostProvisionGooglePaySuccessMockResponse(MarqetaProvisionGooglePayRequest request)
        {
            var mockedResponse = new MarqetaProvisionGooglePayResponse()
            {
                CreatedTime = DateTime.Now,
                LastModifiedTime = DateTime.Now,
                // Return the card token sent in the request.
                CardToken = request.CardToken,
                PushTokenizeRequestData = new MarqetaProvisionGooglePayTokenizationData
                {
                    DisplayName = "Visa Card",
                    LastDigits = "3264",
                    Network = "Visa",
                    TokenServiceProvider = "TOKEN_PROVIDER_VISA",
                    OpaquePaymentCard = $"{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}",
                    UserAddress = new MarqetaProvisionGooglePayUserAddress
                    {
                        Name = "Test User",
                        Address1 = "256 West Data Dr",
                        Address2 = "",
                        City = "Draper",
                        State = "UT",
                        ZipCode = "84020",
                        Country = "",
                        Phone = "+18778981970"
                    }
                }
            };

            return mockedResponse;
        }
    }
}
