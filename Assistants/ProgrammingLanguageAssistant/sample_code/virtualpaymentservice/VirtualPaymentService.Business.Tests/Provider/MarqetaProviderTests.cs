using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Common.AutoMapper;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Provider;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Requests.Marqeta;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Responses.Marqeta;
using VirtualPaymentService.Model.Types;

namespace VirtualPaymentService.Business.Tests.Provider
{

    [TestFixture]
    public class MarqetaProviderTests
    {
        #region Field
        private MarqetaProvider _provider;
        private Mock<IMarqetaCommercialService> _mockCommercialService;
        private Mock<IMarqetaConsumerService> _mockConsumerService;
        private Mock<ILogger<MarqetaProvider>> _mockLogger;
        private AppSettings _appsettings;
        #endregion Field

        #region Setup
        [SetUp]
        public void SetUp()
        {
            _mockCommercialService = new Mock<IMarqetaCommercialService>();
            _mockConsumerService = new Mock<IMarqetaConsumerService>();
            _mockLogger = new Mock<ILogger<MarqetaProvider>>();

            // Gets appsettings.json, found in the test project.
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Bind the appsettings.
            _appsettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(_appsettings);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DigitalWalletProfile>();
                cfg.AddProfile<VirtualCardProfile>();
            });
            config.AssertConfigurationIsValid();

            _provider = new MarqetaProvider(_mockCommercialService.Object, _mockConsumerService.Object, _mockLogger.Object, new Mapper(config), _appsettings);
        }
        #endregion Setup

        #region CancelCardAsync
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task Given_CancelCardAsync_When_ServiceCallIsSuccessful_Then_ReturnsTrue(ProductType productType)
        {
            // Arrange
            var cancelReq = new CancelVCardRequest
            {
                ReferenceId = "random1234"
            };

            _mockCommercialService
                .Setup(s => s.PostTransitionVCardAsync(It.IsAny<MarqetaTransitionCardRequest>()))
                .ReturnsAsync(new MarqetaTransitionCardResponse())
                .Callback<MarqetaTransitionCardRequest>(request =>
                {
                    Assert.AreEqual( MarqetaCardState.TERMINATED,request.CardStatus);
                    Assert.AreEqual(cancelReq.ReferenceId, request.CardToken);
                });
            _mockConsumerService
                .Setup(s => s.PostTransitionVCardAsync(It.IsAny<MarqetaTransitionCardRequest>()))
                .ReturnsAsync(new MarqetaTransitionCardResponse())
                .Callback<MarqetaTransitionCardRequest>(request =>
                {
                    Assert.AreEqual( MarqetaCardState.TERMINATED,request.CardStatus);
                    Assert.AreEqual(cancelReq.ReferenceId, request.CardToken);
                });

            // Act
            var isSuccess = await _provider.CancelCardAsync(cancelReq, productType);

            // Assert
            Assert.IsTrue(isSuccess);
        }

        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task Given_CancelCardAsync_When_ServiceCallIsUnsuccessful_Then_ReturnsFalse(ProductType productType)
        {
            // Arrange
            _mockCommercialService
                .Setup(s => s.PostTransitionVCardAsync(It.IsAny<MarqetaTransitionCardRequest>()))
                .ThrowsAsync(new HttpRequestException());
            _mockConsumerService
                .Setup(s => s.PostTransitionVCardAsync(It.IsAny<MarqetaTransitionCardRequest>()))
                .ThrowsAsync(new HttpRequestException());

            // Act
            var isSuccess = await _provider.CancelCardAsync(new CancelVCardRequest(), productType);

            // Assert
            Assert.IsFalse(isSuccess);
            _mockLogger.Verify(l =>
                l.Log(LogLevel.Error,
                It.IsAny<string>(),
                It.IsAny<HttpRequestException>(),
                It.IsAny<IDictionary<string, object>>())
             );
        }
        #endregion CancelCardAsync

        #region PingAsync Test
        [Test]
        public async Task PingAsyncReturnsTrueWhenServiceCallIs_Successful()
        {
            // Assemble
            _mockCommercialService
                .Setup(service => service.PingAsync())
                .ReturnsAsync(new MarqetaPingResponse { Success = true });
            _mockConsumerService
                .Setup(service => service.PingAsync())
                .ReturnsAsync(new MarqetaPingResponse { Success = true });

            // Act
            var response = await _provider.IsHealthyAsync();

            // Assert
            Assert.IsTrue(response);
        }

        [Test]
        public async Task PingAsyncReturnsFalseWhenServiceCallIs_Unsuccessful()
        {
            // Assemble
            _mockCommercialService
                .Setup(service => service.PingAsync())
                .ReturnsAsync(new MarqetaPingResponse { Success = false });
            _mockConsumerService
                .Setup(service => service.PingAsync())
                .ReturnsAsync(new MarqetaPingResponse { Success = false });

            // Act
            var response = await _provider.IsHealthyAsync();

            // Assert
            Assert.IsFalse(response);
        }

        [Test]
        public async Task PingAsyncReturnsFalse_AndLogs_WhenAnExceptionOccurs()
        {
            // Assemble
            var expectedException = new Exception("Ah");
            _mockCommercialService
                .Setup(service => service.PingAsync())
                .ThrowsAsync(expectedException);
            _mockConsumerService
                .Setup(service => service.PingAsync())
                .ThrowsAsync(expectedException);

            // Act
            var response = await _provider.IsHealthyAsync();

            // Assert
            _mockLogger
                .Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), expectedException, It.IsAny<IDictionary<string, object>>()), Times.Exactly(2));
            Assert.IsFalse(response);
        }
        #endregion PingAsync Test

        #region GetApplePayTokenizationDataAsync Test
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task PostProvisionApplePayAsync_Returns_ProperlyFormatted_ApplePayTokenizationResponse(ProductType productType)
        {
            // Assemble
            var request = new ApplePayTokenizationRequest
            {
                LeaseId = 12345678,
                Data = new ApplePayProvisioningData
                {
                    CardToken = "Token",
                    DeviceType = DeviceType.MOBILE_PHONE,
                    ProvisioningAppVersion = "1.2.3",
                    LeafCertificate = "Leaf",
                    SubCACertificate = "Sub",
                    Nonce = "Nonce",
                    NonceSignature = "Sig"
                }
            };

            var expected = new MarqetaProvisionApplePayResponse
            {
                CardToken = "My Card Token",
                EncryptedPassData = "Encrypted data",
                ActivationData = "Activated!",
                EphemeralPublicKey = "Ephemeral"
            };

            _mockCommercialService
                .Setup(service => service.PostProvisionApplePayAsync(It.IsAny<MarqetaProvisionApplePayRequest>()))
                .ReturnsAsync(expected);
            _mockConsumerService
                .Setup(service => service.PostProvisionApplePayAsync(It.IsAny<MarqetaProvisionApplePayRequest>()))
                .ReturnsAsync(expected);

            // Act
            ApplePayTokenizationResponse response = await _provider.GetApplePayTokenizationDataAsync(request.Data, productType);

            // Assert
            Assert.AreEqual(expected.CardToken, response.CardToken);
            Assert.AreEqual(expected.EncryptedPassData, response.EncryptedPassData);
            Assert.AreEqual(expected.ActivationData, response.ActivationData);
            Assert.AreEqual(expected.EphemeralPublicKey, response.EphemeralPublicKey);
        }

        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task GetApplePayTokenizationDataAsync_ReturnsSuccessFalseAndLogsError_WhenExceptionThrown(ProductType productType)
        {
            // Assemble
            _mockCommercialService
                .Setup(service => service.PostProvisionApplePayAsync(It.IsAny<MarqetaProvisionApplePayRequest>()))
                .ThrowsAsync(new Exception());
            _mockConsumerService
                .Setup(service => service.PostProvisionApplePayAsync(It.IsAny<MarqetaProvisionApplePayRequest>()))
                .ThrowsAsync(new Exception());

            // Act
            var response = await _provider.GetApplePayTokenizationDataAsync(new ApplePayTokenizationRequest().Data, productType);

            // Assert
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.IsNull(response.ActivationData);
        }
        #endregion GetApplePayTokenizationDataAsync Test

        #region Google Pay Tokenization Tests
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task PostProvisionGooglePayAsync_Returns_ProperlyFormatted_GooglePayTokenizationResponse(ProductType productType)
        {
            // Assemble
            var request = new GooglePayTokenizationRequest()
            {
                LeaseId = 12345678,
                Data = new GooglePayProvisioningData
                {
                    CardToken = "Token",
                    DeviceType = DeviceType.MOBILE_PHONE,
                    ProvisioningAppVersion = "1.2.3",
                    WalletAccountId = "ae25OGhjZTk2dsr452dgsr51",
                    DeviceId = "W85OGhjZTk2dsr452dgsr51j"
                }
            };

            var expected = new MarqetaProvisionGooglePayResponse
            {
                CardToken = "My Card Token",
                CreatedTime = DateTime.Parse("2019-11-06T22:43:20Z"),
                LastModifiedTime = DateTime.Parse("2019-11-06T22:43:20Z"),
                PushTokenizeRequestData = new MarqetaProvisionGooglePayTokenizationData
                {
                    DisplayName = "Visa Card",
                    LastDigits = "3264",
                    Network = "Visa",
                    TokenServiceProvider = "TOKEN_PROVIDER_VISA",
                    OpaquePaymentCard = "eyJraWQiOiIxVjMwT1ZCUTNUMjRZMVFBVFRRUza",
                    UserAddress = new MarqetaProvisionGooglePayUserAddress
                    {
                        Name = "John Doe",
                        Address1 = "180 Grand Ave",
                        Address2 = "Suite 500",
                        City = "Oakland",
                        State = "CA",
                        PostalCode = "01234",
                        ZipCode = null,
                        Country = "US",
                        Phone = "5105551212"
                    }
                }
            };

            _mockCommercialService
                .Setup(service => service.PostProvisionGooglePayAsync(It.IsAny<MarqetaProvisionGooglePayRequest>()))
                .ReturnsAsync(expected);
            _mockConsumerService
                .Setup(service => service.PostProvisionGooglePayAsync(It.IsAny<MarqetaProvisionGooglePayRequest>()))
                .ReturnsAsync(expected);

            // Act
            var response = await _provider.GetGooglePayTokenizationDataAsync(request.Data, productType);

            // Assert
            Assert.IsInstanceOf(typeof(GooglePayTokenizationResponse), response);

            Assert.IsNotNull(response);
            Assert.AreEqual(expected.CardToken, response.CardToken);

            Assert.IsNotNull(response.PushTokenizeRequestData);
            Assert.AreEqual(expected.PushTokenizeRequestData.OpaquePaymentCard, response.PushTokenizeRequestData.OpaquePaymentCard);

            Assert.IsNotNull(response.PushTokenizeRequestData.UserAddress);
            Assert.AreEqual(expected.PushTokenizeRequestData.UserAddress.PostalCode, response.PushTokenizeRequestData.UserAddress.PostalCode);
        }

        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task PostProvisionGooglePayAsync_WhenResponseZipPopulated_ThenRepsonsePostalCodePopulated(ProductType productType)
        {
            // Assemble
            var request = new GooglePayTokenizationRequest()
            {
                LeaseId = 12345678,
                Data = new GooglePayProvisioningData
                {
                    CardToken = "Token",
                    DeviceType = DeviceType.MOBILE_PHONE,
                    ProvisioningAppVersion = "1.2.3",
                    WalletAccountId = "ae25OGhjZTk2dsr452dgsr51",
                    DeviceId = "W85OGhjZTk2dsr452dgsr51j"
                }
            };

            var expected = new MarqetaProvisionGooglePayResponse
            {
                CardToken = "My Card Token",
                CreatedTime = DateTime.Parse("2019-11-06T22:43:20Z"),
                LastModifiedTime = DateTime.Parse("2019-11-06T22:43:20Z"),
                PushTokenizeRequestData = new MarqetaProvisionGooglePayTokenizationData
                {
                    DisplayName = "Visa Card",
                    LastDigits = "3264",
                    Network = "Visa",
                    TokenServiceProvider = "TOKEN_PROVIDER_VISA",
                    OpaquePaymentCard = "eyJraWQiOiIxVjMwT1ZCUTNUMjRZMVFBVFRRUza",
                    UserAddress = new MarqetaProvisionGooglePayUserAddress
                    {
                        Name = "John Doe",
                        Address1 = "180 Grand Ave",
                        Address2 = "Suite 500",
                        City = "Oakland",
                        State = "CA",
                        PostalCode = null,
                        ZipCode = "67890",
                        Country = "US",
                        Phone = "5105551212"
                    }
                }
            };

            _mockCommercialService
                .Setup(service => service.PostProvisionGooglePayAsync(It.IsAny<MarqetaProvisionGooglePayRequest>()))
                .ReturnsAsync(expected);
            _mockConsumerService
                .Setup(service => service.PostProvisionGooglePayAsync(It.IsAny<MarqetaProvisionGooglePayRequest>()))
                .ReturnsAsync(expected);

            // Act
            var response = await _provider.GetGooglePayTokenizationDataAsync(request.Data, productType);

            // Assert
            Assert.AreEqual(expected.PushTokenizeRequestData.UserAddress.ZipCode, response.PushTokenizeRequestData.UserAddress.PostalCode);
        }

        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task PostProvisionGooglePayAsync_WhenResponseZipAndPostalCodePopulated_ThenRepsonsePostalCodePopulated(ProductType productType)
        {
            // Assemble
            var request = new GooglePayTokenizationRequest()
            {
                LeaseId = 12345678,
                Data = new GooglePayProvisioningData
                {
                    CardToken = "Token",
                    DeviceType = DeviceType.MOBILE_PHONE,
                    ProvisioningAppVersion = "1.2.3",
                    WalletAccountId = "ae25OGhjZTk2dsr452dgsr51",
                    DeviceId = "W85OGhjZTk2dsr452dgsr51j"
                }
            };

            var expected = new MarqetaProvisionGooglePayResponse
            {
                CardToken = "My Card Token",
                CreatedTime = DateTime.Parse("2019-11-06T22:43:20Z"),
                LastModifiedTime = DateTime.Parse("2019-11-06T22:43:20Z"),
                PushTokenizeRequestData = new MarqetaProvisionGooglePayTokenizationData
                {
                    DisplayName = "Visa Card",
                    LastDigits = "3264",
                    Network = "Visa",
                    TokenServiceProvider = "TOKEN_PROVIDER_VISA",
                    OpaquePaymentCard = "eyJraWQiOiIxVjMwT1ZCUTNUMjRZMVFBVFRRUza",
                    UserAddress = new MarqetaProvisionGooglePayUserAddress
                    {
                        Name = "John Doe",
                        Address1 = "180 Grand Ave",
                        Address2 = "Suite 500",
                        City = "Oakland",
                        State = "CA",
                        PostalCode = "01234",
                        ZipCode = "67890",
                        Country = "US",
                        Phone = "5105551212"
                    }
                }
            };

            _mockCommercialService
                .Setup(service => service.PostProvisionGooglePayAsync(It.IsAny<MarqetaProvisionGooglePayRequest>()))
                .ReturnsAsync(expected);
            _mockConsumerService
                .Setup(service => service.PostProvisionGooglePayAsync(It.IsAny<MarqetaProvisionGooglePayRequest>()))
                .ReturnsAsync(expected);

            // Act
            var response = await _provider.GetGooglePayTokenizationDataAsync(request.Data, productType);

            // Assert
            Assert.AreEqual(expected.PushTokenizeRequestData.UserAddress.PostalCode, response.PushTokenizeRequestData.UserAddress.PostalCode);
        }

        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task GetGooglePayTokenizationDataAsync_LogsError_WhenExceptionThrown(ProductType productType)
        {
            // Assemble
            _mockCommercialService
                .Setup(service => service.PostProvisionGooglePayAsync(It.IsAny<MarqetaProvisionGooglePayRequest>()))
                .ThrowsAsync(new Exception());
            _mockConsumerService
                .Setup(service => service.PostProvisionGooglePayAsync(It.IsAny<MarqetaProvisionGooglePayRequest>()))
                .ThrowsAsync(new Exception());

            // Act
            var response = await _provider.GetGooglePayTokenizationDataAsync(new GooglePayTokenizationRequest().Data, productType);

            // Assert
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.IsNull(response.PushTokenizeRequestData);
        }
        #endregion Google Pay Tokenization Tests

        #region GetDigitalWalletTokensByVCardAsync Tests
        /// <summary>
        /// Tests the mapping code between <see cref="MarqetaDigitalWalletTokensForCardResponse"/> and
        /// <see cref="DigitalWalletTokenResponse"/> for no wallet tokens located at Marqeta.
        /// </summary>
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task GetDigitalWalletTokensByVCardAsync_WhenHttpSuccessWithNoWalletTokensFound_ThenReturnsMappedResponse(ProductType productType)
        {
            // Assemble
            string expected = "{\"digitalWalletTokens\":[]}";

            // Response from Marqeta when card token exists but no wallet tokens are found.
            string noTokensReturnedFromMarqeta = "{\"count\":0,\"start_index\":0,\"end_index\":0,\"is_more\":false,\"data\":[]}";

            _mockCommercialService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ReturnsAsync(JsonSerializer.Deserialize<MarqetaDigitalWalletTokensForCardResponse>(noTokensReturnedFromMarqeta));
            _mockConsumerService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ReturnsAsync(JsonSerializer.Deserialize<MarqetaDigitalWalletTokensForCardResponse>(noTokensReturnedFromMarqeta));

            // Setup just like the defaults the service will use during runtime.
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            // Act
            var actual = await _provider.GetDigitalWalletTokensByVCardAsync("CardTokenValue", productType);

            // Assert
            Assert.AreEqual(expected, JsonSerializer.Serialize(actual, jsonOptions));
        }

        /// <summary>
        /// Tests the mapping code between <see cref="MarqetaDigitalWalletTokensForCardResponse"/> and
        /// <see cref="DigitalWalletTokenResponse"/> when wallet tokens located at Marqeta.
        /// </summary>
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task GetDigitalWalletTokensByVCardAsync_WhenHttpSuccessWithWalletTokensFound_ThenReturnsMappedResponse(ProductType productType)
        {
            // Assemble
            string expected = Properties.Resources.MarqetaCardTokenTestData_ResponseWithFiveTokens_ExpectedMappedResult;

            // Mocked large response from Marqeta when card token exists and locates 5 tokens with all possible 
            // status values allowing the mapping to be fully tested.
            string tokensReturnedFromMarqeta = Properties.Resources.MarqetaCardTokenTestData_ResponseWithFiveTokens_SourceData;

            _mockCommercialService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ReturnsAsync(JsonSerializer.Deserialize<MarqetaDigitalWalletTokensForCardResponse>(tokensReturnedFromMarqeta));
            _mockConsumerService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ReturnsAsync(JsonSerializer.Deserialize<MarqetaDigitalWalletTokensForCardResponse>(tokensReturnedFromMarqeta));

            // Setup just like the defaults the service will use during runtime.
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            // Act
            var actual = await _provider.GetDigitalWalletTokensByVCardAsync("CardTokenValue", productType);

            // Assert
            Assert.AreEqual(expected, JsonSerializer.Serialize(actual, jsonOptions));
        }

        /// <summary>
        /// If Marqeta does not return the state property in the <see cref="MarqetaDigitalWalletTokensForCardResponse"/> 
        /// the mapping will default the internal state in the response to unknown. The state property in the Marqeta
        /// documentation states this property is conditionally returned.
        /// </summary>
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task GetDigitalWalletTokensByVCardAsync_WhenTokenStatusNotRetuned_ThenUnknownTokenStatusSet(ProductType productType)
        {
            // Assemble
            string expected = "{\"digitalWalletTokens\":[{\"walletToken\":\"WalletTokenValue\",\"walletTokenStatus\":0,\"deviceType\":null,\"cardToken\":null}]}";

            string noTokenStatusReturnedFromMarqeta = "{\"count\":1,\"start_index\":0,\"end_index\":0,\"is_more\":false,\"data\":[{\"token\":\"WalletTokenValue\"}]}";

            _mockCommercialService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ReturnsAsync(JsonSerializer.Deserialize<MarqetaDigitalWalletTokensForCardResponse>(noTokenStatusReturnedFromMarqeta));
            _mockConsumerService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ReturnsAsync(JsonSerializer.Deserialize<MarqetaDigitalWalletTokensForCardResponse>(noTokenStatusReturnedFromMarqeta));

            // Setup just like the defaults the service will use during runtime.
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            // Act
            var actual = await _provider.GetDigitalWalletTokensByVCardAsync("CardTokenValue", productType);

            // Assert
            Assert.AreEqual(expected, JsonSerializer.Serialize(actual, jsonOptions));
        }

        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task GetDigitalWalletTokensByVCardAsync_WhenHttpFailure_ThenLogsException(ProductType productType)
        {
            // Assemble
            _mockCommercialService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("HTTP error from digital wallet provider.", null, HttpStatusCode.NotFound));
            _mockConsumerService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("HTTP error from digital wallet provider.", null, HttpStatusCode.NotFound));

            try
            {
                // Act
                _ = await _provider.GetDigitalWalletTokensByVCardAsync("Card Token", productType);
            }
            catch
            {
                // Assert
                _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<HttpRequestException>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            }
        }

        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public void GetDigitalWalletTokensByVCardAsync_WhenHttpFailure_ThenRethrowsSameException(ProductType productType)
        {
            // Assemble
            _mockCommercialService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("HTTP error from digital wallet provider.", null, HttpStatusCode.NotFound));
            _mockConsumerService
                .Setup(service => service.GetDigitalWalletTokensByCardToken(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("HTTP error from digital wallet provider.", null, HttpStatusCode.NotFound));

            // Act-Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _provider.GetDigitalWalletTokensByVCardAsync("Card Token", productType));
        }
        #endregion GetDigitalWalletTokensByVCardAsync Tests

        #region TransitionDigitalWalletTokenAsync Tests

        [TestCase(ProductType.Commercial, null, 0)]
        [TestCase(ProductType.Consumer, null, 0)]
        [TestCase(ProductType.Commercial, "", 0)]
        [TestCase(ProductType.Consumer, "", 0)]
        [TestCase(ProductType.Commercial, "ErrorCodeReceived", 1)]
        [TestCase(ProductType.Consumer, "ErrorCodeReceived", 1)]
        public async Task TransitionDigitalWalletTokenAsync_Translates_Response_ToAndFrom_MarqetaContracts_Correctly_AndLogsWarningIfErrorCodeExits(
            ProductType productType, string errorCode, int warnlogTimes)
        {
            // Assemble
            var request = new DigitalWalletTokenTransitionRequest
            {
                DigitalWalletToken = new DigitalWalletToken
                { CardToken = "1234", WalletToken = "4321", DeviceType = "MOBILE_PHONE" },
                WalletTransitionReasonCode = "18",
                WalletTokenTransitionStatus = DigitalWalletTokenStatus.Yellow
            };

            var mockExpectedMarqetaRequestInput = new MarqetaWalletTokenTransitionRequest
            {
                DigitalWalletToken = new MarqetaDigitalWalletTokenForTransition { Token = "4321" },
                State = MarqetaDigitalWalletTokenStatus.SUSPENDED,
                ReasonCode = "18"
            };

            var mockMarqetaResponseReturnedForExpectedInput = new MarqetaWalletTokenTransitionResponse()
            {
                WalletTransitionToken = "test_transition_token",
                WalletTransitionType = "state.activated.test",
                State = MarqetaDigitalWalletTokenStatus.ACTIVE,
                FulfillmentStatus = "PROVISIONED_TEST",
                ErrorCode = errorCode,
                ErrorMessage = "reason for error code"
            };

            var expectedResponse = new DigitalWalletTokenTransitionResponse
            {
                WalletTransitionToken = "test_transition_token",
                WalletTransitionType = "state.activated.test",
                WalletTransitionState = DigitalWalletTokenStatus.Green,
                FulfillmentStatus = "PROVISIONED_TEST",
            };

            _mockCommercialService
                .Setup(service =>
                    service.PostDigitalWalletTokenTransitionAsync(
                        It.Is<MarqetaWalletTokenTransitionRequest>(r =>
                            r.DigitalWalletToken.Token == mockExpectedMarqetaRequestInput.DigitalWalletToken.Token &&
                            r.ReasonCode == mockExpectedMarqetaRequestInput.ReasonCode &&
                            r.State == mockExpectedMarqetaRequestInput.State)))
                .ReturnsAsync(mockMarqetaResponseReturnedForExpectedInput);
            _mockConsumerService
                .Setup(service =>
                    service.PostDigitalWalletTokenTransitionAsync(
                        It.Is<MarqetaWalletTokenTransitionRequest>(r =>
                            r.DigitalWalletToken.Token == mockExpectedMarqetaRequestInput.DigitalWalletToken.Token &&
                            r.ReasonCode == mockExpectedMarqetaRequestInput.ReasonCode &&
                            r.State == mockExpectedMarqetaRequestInput.State)))
                .ReturnsAsync(mockMarqetaResponseReturnedForExpectedInput);

            // Act
            var actual = await _provider.TransitionDigitalWalletTokenAsync(request, productType);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(expectedResponse.WalletTransitionToken, actual.WalletTransitionToken);
            Assert.AreEqual(expectedResponse.WalletTransitionState, actual.WalletTransitionState);
            Assert.AreEqual(expectedResponse.WalletTransitionType, actual.WalletTransitionType);
            Assert.AreEqual(expectedResponse.FulfillmentStatus, actual.FulfillmentStatus);

            _mockLogger.Verify(
                logger => logger.Log(LogLevel.Warn,
                    It.Is<string>(m =>
                        m.Equals($"Attempt to transition {_provider.Provider} wallet token encountered " +
                        $"{nameof(mockMarqetaResponseReturnedForExpectedInput.ErrorCode)}: '{mockMarqetaResponseReturnedForExpectedInput.ErrorCode}' - " +
                        $"{nameof(mockMarqetaResponseReturnedForExpectedInput.ErrorMessage)}: '{mockMarqetaResponseReturnedForExpectedInput.ErrorMessage}', " +
                        "but was on the configurable allow list to response successfully.")),
                    It.IsAny<HttpRequestException>(), It.IsAny<IDictionary<string, object>>()), Times.Exactly(warnlogTimes));
        }

        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public void TransitionDigitalWalletTokenAsync_WhenHttpFailure_RethrowsSameException(ProductType productType)
        {
            // Assemble
            var request = new DigitalWalletTokenTransitionRequest
            {
                DigitalWalletToken = new DigitalWalletToken
                { CardToken = "1234", WalletToken = "4321", DeviceType = "MOBILE_PHONE" },
                WalletTransitionReasonCode = "18",
                WalletTokenTransitionStatus = DigitalWalletTokenStatus.Yellow
            };

            _mockCommercialService
                .Setup(service => service.PostDigitalWalletTokenTransitionAsync(It.IsAny<MarqetaWalletTokenTransitionRequest>()))
                .ThrowsAsync(new HttpRequestException("HttpRequestException Message.", null, HttpStatusCode.NotFound));
            _mockConsumerService
                .Setup(service => service.PostDigitalWalletTokenTransitionAsync(It.IsAny<MarqetaWalletTokenTransitionRequest>()))
                .ThrowsAsync(new HttpRequestException("HttpRequestException Message.", null, HttpStatusCode.NotFound));

            // Act-Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _provider.TransitionDigitalWalletTokenAsync(request, productType));
        }

        /// <summary>
        /// Note: If these tests break and must be updated due to changes in Messages,
        /// please ensure any alert checking for the related logging messages are updated to capture the new message.
        /// </summary>
        /// <returns></returns>
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task TransitionDigitalWalletTokenAsync_WhenHttpFailure_Logs_Expected_Exception(ProductType productType)
        {
            // Assemble
            var request = new DigitalWalletTokenTransitionRequest
            {
                DigitalWalletToken = new DigitalWalletToken
                { CardToken = "1234", WalletToken = "4321", DeviceType = "MOBILE_PHONE" },
                WalletTransitionReasonCode = "18",
                WalletTokenTransitionStatus = DigitalWalletTokenStatus.Yellow
            };

            _mockCommercialService
                .Setup(service => service.PostDigitalWalletTokenTransitionAsync(It.IsAny<MarqetaWalletTokenTransitionRequest>()))
                .ThrowsAsync(new HttpRequestException("HttpRequestException Message.", null, HttpStatusCode.NotFound));
            _mockConsumerService
                .Setup(service => service.PostDigitalWalletTokenTransitionAsync(It.IsAny<MarqetaWalletTokenTransitionRequest>()))
                .ThrowsAsync(new HttpRequestException("HttpRequestException Message.", null, HttpStatusCode.NotFound));

            // Act-Assert
            try
            {
                _ = await _provider.TransitionDigitalWalletTokenAsync(request, productType);
            }
            catch
            {
                _mockLogger.Verify(
                    logger => logger.Log(LogLevel.Error,
                        It.Is<string>(m =>
                            m.StartsWith("An error occurred while attempting to call Marqeta to transition token for")),
                        It.IsAny<HttpRequestException>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            }
        }

        #endregion TransitionDigitalWalletTokenAsync Tests

        #region UpdateCardUserAsync Tests
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task UpdateCardUserAsync_WhenPhoneAndCountryPassedIn_ThenPhoneCorrectlyFormattedInMarqetaRequest(ProductType productType)
        {
            // Arrange
            var request = new CardUserUpdateRequest
            {
                PhoneNumber = "8015551212",
                PhoneNumberCountryCode = "1"
            };

            // Formatted value created in mapper
            var expected = "+18015551212";

            MarqetaUserPutRequest actual = null;

            _mockCommercialService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                .Callback<string, MarqetaUserPutRequest>(
                    (userToken, request) =>
                    {
                        actual = request;
                    })
                .ReturnsAsync(new MarqetaUserResponse());
            _mockConsumerService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                .Callback<string, MarqetaUserPutRequest>(
                    (userToken, request) =>
                    {
                        actual = request;
                    })
                .ReturnsAsync(new MarqetaUserResponse());

            // Act
            _ = await _provider.UpdateCardUserAsync("UserTokenValue", request, productType);

            // Assert
            Assert.AreEqual(expected, actual.Phone);
        }

        [TestCase("US", "US")]
        [TestCase("CO", "CO")]
        [TestCase("", null)]
        [TestCase(null, null)]
        public async Task UpdateCardUserAsync_WhenCountryCodePassedIn_ThenCountryCodeFormattedInMarqetaRequest(
            string countryCode, string expectedCountryCode)
        {
            // Arrange
            var request = new CardUserUpdateRequest
            {
                PhoneNumber = "8015551212",
                PhoneNumberCountryCode = "1",
                CountryCode = countryCode
            };

            MarqetaUserPutRequest actual = null;

            _mockCommercialService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                .Callback<string, MarqetaUserPutRequest>(
                    (userToken, request) => { actual = request; })
                .ReturnsAsync(new MarqetaUserResponse());
            _mockConsumerService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                .Callback<string, MarqetaUserPutRequest>(
                    (userToken, request) => { actual = request; })
                .ReturnsAsync(new MarqetaUserResponse());

            // Act
            await _provider.UpdateCardUserAsync("UserTokenValue", request, ProductType.Consumer);

            // Assert
            Assert.AreEqual(expectedCountryCode, actual.CountryCode);
        }

        [TestCase("US", "")]
        [TestCase("CO", "")]
        [TestCase("", "")]
        [TestCase(null, "")]
        [TestCase("   ", "The country code must be two characters")]
        [TestCase("A", "The country code must be two characters")]
        [TestCase("12", "The country code must be two characters")]
        [TestCase("AAA", "The country code does not meet the required number of characters")]
        public async Task UpdateCardUserAsync_WhenCountryCodePassedIn_ValidateRequest(
            string countryCode, string validationMessage)
        {
            // Arrange
            var request = new CardUserUpdateRequest
            {
                PhoneNumber = "8015551212",
                PhoneNumberCountryCode = "1",
                CountryCode = countryCode
            };

            // Act
            await _provider.UpdateCardUserAsync("UserTokenValue", request, ProductType.Consumer);

            // Assert
            var validationErrors = ValidateModel(request);

            if (string.IsNullOrWhiteSpace(validationMessage))
            {
                Assert.AreEqual(0, validationErrors.Count);
            }
            else
            {
                Assert.IsTrue(
                    validationErrors.Any(e => e.ErrorMessage != null && e.ErrorMessage.Contains(validationMessage)));
            }
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
        
        /// <summary>
        /// Tests that all HTTP errors returned from Marqeta will keep the same HTTP error returned
        /// with a generic message. An HTTP 400 should be mapped to a 409 since Prog wants a 409 for 
        /// internal PATCH calls that cant find the resource.
        /// </summary>
        [TestCase("Random HTTP 500 error message.", HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError, "An error occurred while updating user for userToken value 123456 passed in.", ProductType.Commercial)]
        [TestCase("Random HTTP 408 error message.", HttpStatusCode.RequestTimeout, HttpStatusCode.RequestTimeout, "An error occurred while updating user for userToken value 123456 passed in.", ProductType.Commercial)]
        [TestCase("Random HTTP 400 error message.", HttpStatusCode.BadRequest, HttpStatusCode.Conflict, "The userToken value 123456 passed in was not located on the virtual card provider network.", ProductType.Commercial)]
        [TestCase("Random HTTP 500 error message.", HttpStatusCode.InternalServerError, HttpStatusCode.InternalServerError, "An error occurred while updating user for userToken value 123456 passed in.", ProductType.Consumer)]
        [TestCase("Random HTTP 408 error message.", HttpStatusCode.RequestTimeout, HttpStatusCode.RequestTimeout, "An error occurred while updating user for userToken value 123456 passed in.", ProductType.Consumer)]
        [TestCase("Random HTTP 400 error message.", HttpStatusCode.BadRequest, HttpStatusCode.Conflict, "The userToken value 123456 passed in was not located on the virtual card provider network.", ProductType.Consumer)]
        public void UpdateCardUserAsync_WhenHttpExceptionReturnedFromMarqeta_ThenReturnSameResponseCodeUnlessHttp400(
            string messageFromHttpCall, HttpStatusCode statusCodeFromHttpCall, HttpStatusCode expectedHttpStatusCode, string expectedMessageReturned, ProductType productType)
        {
            // Arrange
            _mockCommercialService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                .ThrowsAsync(new HttpRequestException(messageFromHttpCall, null, statusCodeFromHttpCall));
            _mockConsumerService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                .ThrowsAsync(new HttpRequestException(messageFromHttpCall, null, statusCodeFromHttpCall));

            // Act
            HttpRequestException actual = Assert.ThrowsAsync<HttpRequestException>(async () => await _provider.UpdateCardUserAsync("123456", new CardUserUpdateRequest(), productType));

            // Assert
            Assert.AreEqual(expectedMessageReturned, actual.Message);
            Assert.AreEqual(expectedHttpStatusCode, actual.StatusCode);
        }
        #endregion UpdateCardUserAsync Tests

        #region GetCardUserAsync Tests
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task GetCardUserAsync_WhenCardUserFound_ThenMappedUserResponseReturned(ProductType productType)
        {
            // Arrange
            var expected = new CardUserResponse()
            {
                PhoneNumber = "+18016525555",
                FirstName = "TestFirst",
                LastName = "TestLast",
                Address1 = "1234 Main",
                Address2 = "#300",
                City = "South Jordan",
                State = "UT",
                PostalCode = "84095",
                Active = true,
                CreatedTime = DateTime.UtcNow,
                LastModifiedTime = DateTime.UtcNow,
            };

            var marqetaUserResponse = new MarqetaUserResponse()
            {
                Phone = expected.PhoneNumber,
                FirstName = expected.FirstName,
                LastName = expected.LastName,
                Address1 = expected.Address1,
                Address2 = expected.Address2,
                City = expected.City,
                State = expected.State,
                Zip = expected.PostalCode,
                Active = true,
                PostalCode = expected.PostalCode,
                CreatedTime = expected.CreatedTime,
                LastModifiedTime = expected.LastModifiedTime
            };

            _mockCommercialService
                .Setup(service => service.GetUserAsync(It.IsAny<string>()))
                .ReturnsAsync(marqetaUserResponse);
            _mockConsumerService
                .Setup(service => service.GetUserAsync(It.IsAny<string>()))
                .ReturnsAsync(marqetaUserResponse);

            // Act 
            var actual = await _provider.GetCardUserAsync("123456", productType);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase("HTTP 500 error message.", HttpStatusCode.InternalServerError, "HTTP 500 error message.", ProductType.Commercial)]
        [TestCase("HTTP 404 error message.", HttpStatusCode.NotFound, "The userToken value 123456 passed in was not located on the Marqeta Commercial card network.", ProductType.Commercial)]
        [TestCase("HTTP 500 error message.", HttpStatusCode.InternalServerError, "HTTP 500 error message.", ProductType.Consumer)]
        [TestCase("HTTP 404 error message.", HttpStatusCode.NotFound, "The userToken value 123456 passed in was not located on the Marqeta Consumer card network.", ProductType.Consumer)]
        public void GetCardUserAsync_WhenHttpExceptionThrown_ThenHttpExceptionReturned(
            string messageFromHttpCall,
            HttpStatusCode expectedHttpStatusCode,
            string expectedMessageReturned,
            ProductType productType)
        {
            // Arrange
            _mockCommercialService
                .Setup(service => service.GetUserAsync(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException(messageFromHttpCall, null, expectedHttpStatusCode));
            _mockConsumerService
                .Setup(service => service.GetUserAsync(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException(messageFromHttpCall, null, expectedHttpStatusCode));

            // Act 
            HttpRequestException actual = Assert.ThrowsAsync<HttpRequestException>(async () => await _provider.GetCardUserAsync("123456", productType));

            // Assert
            Assert.AreEqual(expectedMessageReturned, actual.Message);
            Assert.AreEqual(expectedHttpStatusCode, actual.StatusCode);
        }

        #endregion GetCardUserAsync Tests

        #region CreateCardAsync Tests
        [Test]
        public async Task CreateCardAsync_WhenUserDoesNotExist_ThenOnlyPostActionWasCalled()
        {
            // Arrange
            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            // Act
            _ = await _provider.CreateCardAsync(new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 123
            },
            ProductType.Commercial);

            // Assert
            _mockCommercialService.Verify(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()), Times.Once());
            _mockCommercialService.Verify(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPostRequest>()), Times.Never());
            _mockLogger.Verify(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<IDictionary<string, object>>()), Times.Never());
        }

        [TestCase(Constants.PuertoRicoCode, Constants.PuertoRicoCode, Constants.PuertoRicoCode)]
        [TestCase(Constants.PuertoRicoCode, Constants.UnitedStatesCode, Constants.PuertoRicoCode)]
        [TestCase(Constants.PuertoRicoCode, "", Constants.PuertoRicoCode)]
        [TestCase("", Constants.PuertoRicoCode, Constants.PuertoRicoCode)]
        [TestCase(Constants.VirginIslandsCode, Constants.VirginIslandsCode, Constants.VirginIslandsCode)]
        [TestCase(Constants.VirginIslandsCode, Constants.UnitedStatesCode, Constants.VirginIslandsCode)]
        [TestCase("", Constants.VirginIslandsCode, Constants.VirginIslandsCode)]
        [TestCase(Constants.VirginIslandsCode, "", Constants.VirginIslandsCode)]
        [TestCase("UT", Constants.UnitedStatesCode, Constants.UnitedStatesCode)]
        [TestCase("CA", Constants.UnitedStatesCode, Constants.UnitedStatesCode)]
        [TestCase("", Constants.UnitedStatesCode, Constants.UnitedStatesCode)]
        [TestCase(null, Constants.UnitedStatesCode, Constants.UnitedStatesCode)]
        [TestCase(null, null, null)]
        [TestCase("", "", null)]
        public async Task CreateCardAsync_WhenUserStateIsPuertoRico_ThenCountryIsPuertoRico(string state,
            string countryCode, string expectedCountryCode)
        {
            // Arrange
            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            // Act
            _ = await _provider.CreateCardAsync(new VirtualCardRequest()
                {
                    VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                    LeaseId = 123456,
                    StoreId = 123,
                    User = new CardUserUpdateRequest
                    {
                        State = state,
                        CountryCode = countryCode
                    }
                },
                ProductType.Commercial);

            // Assert
            _mockCommercialService.Verify(service =>
                service.PostUserAsync(
                    It.Is<MarqetaUserPostRequest>(arg => arg.CountryCode == expectedCountryCode)));
        }

        [Test]
        public async Task CreateCardAsync_WhenUserDoesExist_ThenPostAndPutCalled()
        {
            // Arrange
            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                // Returns a 409 conflict that mimics the error we get from Marqeta when the user already exists.
                .ThrowsAsync(new HttpRequestException("Conflict error", null, HttpStatusCode.Conflict));

            _mockCommercialService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            // Act
            _ = await _provider.CreateCardAsync(new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 123,
                User = new CardUserUpdateRequest() { PhoneNumber = "8016527096", PhoneNumberCountryCode = "1" }
            },
            ProductType.Commercial);

            // Assert
            _mockCommercialService.Verify(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()), Times.Once());
            _mockCommercialService.Verify(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()), Times.Once());
            _mockLogger.Verify(logger => logger.Log(LogLevel.Info, It.IsAny<string>(), null, null), Times.Once());
        }

        [Test]
        public void CreateCardAsync_WhenUserPostFails_ThenHttpExceptionReturned()
        {
            // Arrange
            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                // Returns a 401 mimicking a user/password error
                .ThrowsAsync(new HttpRequestException("Unauthorized user credentials.", null, HttpStatusCode.Unauthorized));

            _mockCommercialService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            // Act
            HttpRequestException actual = Assert.ThrowsAsync<HttpRequestException>(async () => await _provider.CreateCardAsync(new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 123,
                User = new CardUserUpdateRequest() { PhoneNumber = "8016527096", PhoneNumberCountryCode = "1" }
            }, ProductType.Commercial));

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.StatusCode);
            _mockCommercialService.Verify(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()), Times.Once());
            _mockCommercialService.Verify(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()), Times.Never());
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<HttpRequestException>(), null), Times.Exactly(2));
        }

        [Test]
        public void CreateCardAsync_WhenUserPostException_ThenExceptionReturned()
        {
            // Arrange
            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .ThrowsAsync(new Exception("Random exception!"));

            // Act
            _ = Assert.ThrowsAsync<Exception>(async () => await _provider.CreateCardAsync(new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 123,
                User = new CardUserUpdateRequest() { PhoneNumber = "8016527096", PhoneNumberCountryCode = "1" }
            }, ProductType.Commercial));

            // Assert
            _mockCommercialService.Verify(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()), Times.Once());
            _mockCommercialService.Verify(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()), Times.Never());
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<Exception>(), null), Times.Exactly(2));
        }

        [Test]
        public void CreateCardAsync_WhenUserPutFails_ThenHttpExceptionReturned()
        {
            // Arrange
            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                // Returns a 409 conflict that mimics the error we get from Marqeta when the user already exists.
                .ThrowsAsync(new HttpRequestException("Conflict error", null, HttpStatusCode.Conflict));

            _mockCommercialService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                // Returns a 401 mimicking a user/password error
                .ThrowsAsync(new HttpRequestException("Unauthorized user credentials.", null, HttpStatusCode.Unauthorized));

            // Act
            HttpRequestException actual = Assert.ThrowsAsync<HttpRequestException>(async () => await _provider.CreateCardAsync(new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 123,
                User = new CardUserUpdateRequest() { PhoneNumber = "8016527096", PhoneNumberCountryCode = "1" }
            }, ProductType.Commercial));

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.StatusCode);
            _mockCommercialService.Verify(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()), Times.Once());
            _mockCommercialService.Verify(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()), Times.Once());
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<HttpRequestException>(), null), Times.Once());
        }

        [TestCase(50)]
        [TestCase(36)]
        [TestCase(12)]
        public async Task CreateCardAsync_WhenCardCreated_ThenUniqueCardTokenGenerated(int maxTokenLength)
        {
            // Arrange
            MarqetaCardRequest actual = null;

            _appsettings.MarqetaCardTokenMaxLength = maxTokenLength;

            var request = new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 7894561
            };

            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .Callback<MarqetaCardRequest>(
                    (request) =>
                    {
                        actual = request;
                    })
                .ReturnsAsync(new MarqetaCardResponse());

            // Act
            _ = await _provider.CreateCardAsync(request, ProductType.Commercial);

            // Assert
            Assert.IsTrue(actual.Token.Length <= maxTokenLength);
            Assert.IsTrue(actual.Token.StartsWith($"{request.StoreId}-"));
        }

        [Test]
        public void CreateCardAsync_WhenTokenMaxLengthInvaild_ThenArgumentExceptionThrown()
        {
            // Arrange
            _appsettings.MarqetaCardTokenMaxLength = 0;

            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            // Act
            ArgumentException actual = Assert.ThrowsAsync<ArgumentException>(async () => await _provider.CreateCardAsync(new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 7894561
            }, ProductType.Commercial));

            // Assert
            Assert.AreEqual("The setting MarqetaCardTokenMaxLength in appsetting.json must be greater than zero!", actual.Message);
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(2, 2)]
        [TestCase(5, 5)]
        public async Task CreateCardAsync_WhenCalculatingDaysToActive_ThenRangeConditionHonored(int daysActiveTo, int expectedDaysAddedToDate)
        {
            // Arrange
            var request = new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 7894561,
                ActiveToBufferMinutes = 90,
                DaysActiveTo = daysActiveTo
            };

            var expectedDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time")
                .AddMinutes(request.ActiveToBufferMinutes)
                .AddDays(expectedDaysAddedToDate);

            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            // Act
            var actual = await _provider.CreateCardAsync(request, ProductType.Commercial);

            // Assert
            Assert.IsTrue(actual.Card.ActiveToDate >= expectedDateTime.AddSeconds(-2) && actual.Card.ActiveToDate < expectedDateTime.AddSeconds(2));
        }

        [TestCase(MarqetaCardState.ACTIVE, CardStatus.Open)]
        [TestCase(MarqetaCardState.TERMINATED, CardStatus.Cancelled)]
        [TestCase(MarqetaCardState.UNACTIVATED, CardStatus.Closed)]
        [TestCase(MarqetaCardState.SUSPENDED, CardStatus.Closed)]
        [TestCase(MarqetaCardState.LIMITED, CardStatus.Error)]
        [TestCase(MarqetaCardState.UNSUPPORTED, CardStatus.Error)]
        public async Task CreateCardAsync_WhenMarqetaCardStatusReturned_ThenInternalCardStatusMapped(MarqetaCardState cardState, CardStatus expected)
        {
            // Arrange
            var request = new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 7894561
            };

            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse() { State = cardState });

            // Act
            var actual = await _provider.CreateCardAsync(request, ProductType.Commercial);

            // Assert
            Assert.AreEqual(expected, actual.Card.VCardStatusId);
        }

        [TestCase(ProductType.Commercial, 1, 0)]
        [TestCase(ProductType.Consumer, 0, 1)]
        public async Task CreateCardAsync_WhenProductTypePassedIn_ThenCorrectHttpServicesCalled(ProductType productType, int commercialServiceCalled, int consumerServiceCalled)
        {
            // Arrange
            var request = new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 7894561,
                User = new CardUserUpdateRequest { PhoneNumber = "8015551212" }
            };

            _mockCommercialService
            .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
            .ReturnsAsync(new MarqetaUserResponse());

            _mockConsumerService
            .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
            .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            _mockConsumerService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            _mockConsumerService
                .Setup(service => service.PostPinControlTokenAsync(It.IsAny<MarqetaPinControlTokenRequest>()))
                .ReturnsAsync(new MarqetaPinControlTokenResponse() { ControlToken = "TestPinControlTokenValue" });

            // Act
            _ = await _provider.CreateCardAsync(request, productType);

            // Assert
            _mockCommercialService.Verify(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()), Times.Exactly(commercialServiceCalled));
            _mockCommercialService.Verify(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()), Times.Exactly(commercialServiceCalled));
            _mockConsumerService.Verify(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()), Times.Exactly(consumerServiceCalled));
            _mockConsumerService.Verify(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()), Times.Exactly(consumerServiceCalled));
        }

        [TestCase(ProductType.Null)]
        public void CreateCardAsync_WhenInvaildProductTypePassedIn_ThenNotSupportedExceptionThrown(ProductType productType)
        {
            // Arrange
            var request = new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 7894561
            };

            _appsettings.CardProviderSettings.Add(new CardProviderSetting
            {
                ProductType = productType,
                CardProvider = VirtualCardProviderNetwork.Marqeta
            });

            // Act
            NotSupportedException actual = Assert.ThrowsAsync<NotSupportedException>(async () => await _provider.CreateCardAsync(request, productType));

            // Assert
            actual.Message.Should().Contain("is not currently supported! There may be an issue in the appsettings.json for the combination of provider");
        }

        [TestCase(ProductType.Consumer, "no_kyc_required")]
        [TestCase(ProductType.Commercial, null)]
        public async Task CreateCardAsync_WhenAccountHolderGroupTokenEvaluated_ThenPostRequestUpdated(ProductType productType, string expectedAccountHolderGroupToken)
        {
            // Arrange
            var request = new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 7894561,
                User = new CardUserUpdateRequest { PhoneNumber = "8015551212" }
            };

            MarqetaUserPostRequest actual = null;

            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .Callback<MarqetaUserPostRequest>(
                    (request) =>
                    {
                        actual = request;
                    })
                .ReturnsAsync(new MarqetaUserResponse());

            _mockConsumerService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .Callback<MarqetaUserPostRequest>(
                    (request) =>
                    {
                        actual = request;
                    })
                .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            _mockConsumerService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            _mockConsumerService
                .Setup(service => service.PostPinControlTokenAsync(It.IsAny<MarqetaPinControlTokenRequest>()))
                .ReturnsAsync(new MarqetaPinControlTokenResponse() { ControlToken = "TestPinControlTokenValue" });

            // Act
            _ = await _provider.CreateCardAsync(request, productType);

            // Assert
            actual.AccountHolderGroupToken.Should().Be(expectedAccountHolderGroupToken);
        }

        [TestCase(ProductType.Consumer, "no_kyc_required")]
        [TestCase(ProductType.Commercial, null)]
        public async Task CreateCardAsync_WhenAccountHolderGroupTokenEvaluated_ThenPutRequestUpdated(ProductType productType, string expectedAccountHolderGroupToken)
        {
            // Arrange
            var request = new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 7894561,
                User = new CardUserUpdateRequest { PhoneNumber = "8015551212" }
            };

            // Post must fail with 409 before PUT is called
            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                // Returns a 409 conflict that mimics the error we get from Marqeta when the user already exists.
                .ThrowsAsync(new HttpRequestException("Conflict error", null, HttpStatusCode.Conflict));

            // Post must fail with 409 before PUT is called
            _mockConsumerService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                // Returns a 409 conflict that mimics the error we get from Marqeta when the user already exists.
                .ThrowsAsync(new HttpRequestException("Conflict error", null, HttpStatusCode.Conflict));

            MarqetaUserPutRequest actual = null;

            _mockCommercialService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                .Callback<string, MarqetaUserPutRequest>(
                    (userToken, request) =>
                    {
                        actual = request;
                    })
                .ReturnsAsync(new MarqetaUserResponse());

            _mockConsumerService
                .Setup(service => service.PutUserAsync(It.IsAny<string>(), It.IsAny<MarqetaUserPutRequest>()))
                .Callback<string, MarqetaUserPutRequest>(
                    (userToken, request) =>
                    {
                        actual = request;
                    })
                .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            _mockConsumerService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            _mockConsumerService
                .Setup(service => service.PostPinControlTokenAsync(It.IsAny<MarqetaPinControlTokenRequest>()))
                .ReturnsAsync(new MarqetaPinControlTokenResponse() { ControlToken = "TestPinControlTokenValue" });

            // Act
            _ = await _provider.CreateCardAsync(request, productType);

            // Assert
            actual.AccountHolderGroupToken.Should().Be(expectedAccountHolderGroupToken);
        }

        [TestCase(ProductType.Consumer, true, 0, 0, 1, 1)]
        [TestCase(ProductType.Consumer, false, 0, 0, 0, 0)]
        [TestCase(ProductType.Commercial, true, 1, 1, 0, 0)]
        [TestCase(ProductType.Commercial, false, 0, 0, 0, 0)]
        public async Task CreateCardAsync_WhenSetInitalPinEvaluated_ThenSettingValueHonored(ProductType productType, bool setInitalPinFlagValue,
            int expectedCommericalPutCalled, int expectedCommercialPostCalled,
            int expectedConsumerPutCalled, int expectedConsumerPostCalled)
        {
            // Arrange
            var request = new VirtualCardRequest()
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta,
                LeaseId = 123456,
                StoreId = 7894561,
                User = new CardUserUpdateRequest { PhoneNumber = "8015551212" }
            };

            // Set the initial pin setting
            _appsettings.CardProviderSettings.Find(s => s.CardProvider == VirtualCardProviderNetwork.Marqeta && s.ProductType == productType).SetInitalPin = setInitalPinFlagValue;

            _mockCommercialService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            _mockConsumerService
                .Setup(service => service.PostUserAsync(It.IsAny<MarqetaUserPostRequest>()))
                .ReturnsAsync(new MarqetaUserResponse());

            _mockCommercialService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            _mockConsumerService
                .Setup(service => service.PostCardAsync(It.IsAny<MarqetaCardRequest>()))
                .ReturnsAsync(new MarqetaCardResponse());

            _mockCommercialService
                .Setup(service => service.PostPinControlTokenAsync(It.IsAny<MarqetaPinControlTokenRequest>()))
                .ReturnsAsync(new MarqetaPinControlTokenResponse() { ControlToken = "TestPinControlTokenValue" });

            _mockConsumerService
                .Setup(service => service.PostPinControlTokenAsync(It.IsAny<MarqetaPinControlTokenRequest>()))
                .ReturnsAsync(new MarqetaPinControlTokenResponse() { ControlToken = "TestPinControlTokenValue" });

            _mockCommercialService
                .Setup(service => service.PutPinAsync(It.IsAny<MarqetaPinRequest>()));

            _mockConsumerService
                .Setup(service => service.PutPinAsync(It.IsAny<MarqetaPinRequest>()));

            // Act
            _ = await _provider.CreateCardAsync(request, productType);

            // Assert
            _mockCommercialService.Verify(service => service.PostPinControlTokenAsync(It.IsAny<MarqetaPinControlTokenRequest>()), Times.Exactly(expectedCommercialPostCalled));
            _mockCommercialService.Verify(service => service.PutPinAsync(It.IsAny<MarqetaPinRequest>()), Times.Exactly(expectedCommericalPutCalled));
            _mockConsumerService.Verify(service => service.PostPinControlTokenAsync(It.IsAny<MarqetaPinControlTokenRequest>()), Times.Exactly(expectedConsumerPostCalled));
            _mockConsumerService.Verify(service => service.PutPinAsync(It.IsAny<MarqetaPinRequest>()), Times.Exactly(expectedConsumerPutCalled));
        }

        #endregion CreateCardAsync Tests
    }
}