using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using ProgLeasing.Platform.SecretConfiguration;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Service;
using VirtualPaymentService.Business.Tests.Extensions;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests.Marqeta;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Responses.Marqeta;

namespace VirtualPaymentService.Business.Tests.Service
{
    [TestFixture]
    public class MarqetaConsumerServiceTests
    {
        #region Field
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<ISecretConfigurationService> _mockSecretConfigurationService;
        private AppSettings _appsettings;
        private MarqetaConsumerService _marqetaService;
        #endregion Field

        #region Setup
        [SetUp]
        public void Initialize()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var mockSecretResult = new Mock<ISecretResult>();
            mockSecretResult.Setup(res => res.Success).Returns(true);
            _mockSecretConfigurationService = new Mock<ISecretConfigurationService>();

            // Gets appsettings.json, found in the test project.
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Bind the appsettings.
            _appsettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(_appsettings);
            _appsettings.UseMockedMobileWalletResponse = false;

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _marqetaService = new MarqetaConsumerService(httpClient, _appsettings, _mockSecretConfigurationService.Object);
        }
        #endregion Setup

        #region PostTransitionVCardAsync Tests
        [Test]
        public async Task PostTransitionVCardAsync_WhenCardCreated_ThenCardResponseReturned()
        {
            // Assemble
            var expectedRequest = new MarqetaTransitionCardRequest();
            var expectedResponse = new MarqetaTransitionCardResponse();

            var reqTask = _mockHttpMessageHandler
                    .MockPost()
                    .ReturnsAsync<MarqetaTransitionCardRequest>(HttpStatusCode.Created, expectedResponse);

            // Act
            var actualResponse = await _marqetaService.PostTransitionVCardAsync(expectedRequest);
            var (actualRequest, _) = await reqTask;

            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            actualRequest.Should().BeEquivalentTo(expectedRequest);
        }

        [Test]
        public void PostTransitionVCardAsync_When_UnsuccessfulStatusCode_Then_ShouldThrow()
        {
            // Assemble
            var expectedRequest = new MarqetaTransitionCardRequest();
            var expectedResponse = new MarqetaTransitionCardResponse();

            _ = _mockHttpMessageHandler
                    .MockPost()
                    .ReturnsAsync(HttpStatusCode.InternalServerError, expectedResponse);

            // Act & Assert
            Assert.ThrowsAsync<HttpRequestException>(() => _marqetaService.PostTransitionVCardAsync(expectedRequest));
        }
        #endregion PostTransitionVCardAsync Tests

        #region PingRequest Test
        [Test]
        public async Task MarqetaService_PingRequest_Returns_MarqetaPingResponse()
        {
            // Assemble
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new MarqetaPingResponse()))
                });

            // Act
            var response = await _marqetaService.PingAsync();

            // Assert
            Assert.AreEqual(response.GetType(), typeof(MarqetaPingResponse));
        }
        #endregion PingRequest Test

        #region PostProvisionApplePayAsync Test
        [Test]
        public async Task PostProvisionApplePayAsync_ReturnsDeserialized_MarqetaProvisionApplePayResponse_OnSuccessStatusCode()
        {
            // Assemble
            var expectedResponse = new MarqetaProvisionApplePayResponse
            {
                CardToken = "MyFakeCardToken"
            };
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
                });

            // Act
            var response = await _marqetaService.PostProvisionApplePayAsync(new MarqetaProvisionApplePayRequest());

            // Assert
            Assert.AreEqual(expectedResponse.CardToken, response.CardToken);
        }

        [Test]
        public async Task PostProvisionApplePayAsync_WhenUseMockedMobileWalletResponseTrue_ThenMockedMarqetaProvisionApplePayResponseReturned()
        {
            // Assemble
            var expectedResponse = new MarqetaProvisionApplePayResponse
            {
                CardToken = "MyFakeCardToken"
            };

            // Set the mocked response to be returned.
            _appsettings.UseMockedMobileWalletResponse = true;

            // Act
            var response = await _marqetaService.PostProvisionApplePayAsync(new MarqetaProvisionApplePayRequest() { CardToken = expectedResponse.CardToken });

            // Assert
            Assert.AreEqual(expectedResponse.CardToken, response.CardToken);
        }

        [Test]
        public void PostProvisionApplePayAsync_ThrowsAnHttpException_OnUnsuccessfulStatusCode()
        {
            // Assemble
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _marqetaService.PostProvisionApplePayAsync(new MarqetaProvisionApplePayRequest()));
        }
        #endregion PostProvisionApplePayAsync Test

        #region PostProvisionGooglePayAsync Test
        [Test]
        public async Task PostProvisionGooglePayAsync_ReturnsDeserialized_MarqetaProvisionGooglePayResponse_OnSuccessStatusCode()
        {
            // Assemble
            var expected = new MarqetaProvisionGooglePayResponse
            {
                CardToken = "MyFakeCardToken"
            };
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expected))
                });

            // Act
            var response = await _marqetaService.PostProvisionGooglePayAsync(new MarqetaProvisionGooglePayRequest());

            // Assert
            Assert.IsInstanceOf(typeof(MarqetaProvisionGooglePayResponse), response);
            Assert.IsNotNull(response);
            Assert.AreEqual(expected.CardToken, response.CardToken);
        }

        [Test]
        public async Task PostProvisionGooglePayAsync_WhenUseMockedMobileWalletResponseTrue_ThenMockedMarqetaProvisionGooglePayResponseReturned()
        {
            // Assemble
            var expected = new MarqetaProvisionGooglePayResponse
            {
                CardToken = "MyFakeCardToken"
            };

            // Set the mocked response to be returned.
            _appsettings.UseMockedMobileWalletResponse = true;

            // Act
            var response = await _marqetaService.PostProvisionGooglePayAsync(new MarqetaProvisionGooglePayRequest() { CardToken = expected.CardToken });

            // Assert
            Assert.AreEqual(expected.CardToken, response.CardToken);
        }

        [Test]
        public void PostProvisionGooglePayAsync_ThrowsHttpException_OnUnsuccessfulStatusCode()
        {
            // Assemble
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _marqetaService.PostProvisionGooglePayAsync(new MarqetaProvisionGooglePayRequest()));
        }
        #endregion PostProvisionGooglePayAsync Test

        #region GetDigitalWalletTokensByCardToken Tests
        [Test]
        public async Task GetDigitalWalletTokensByCardToken_WhenCardTokenPassedIn_ThenTokenInjectedIntoRequestUri()
        {
            // Assemble
            string cardToken = "111-444-RR-TTT";
            string url = _appsettings.GetCardProviderSetting(VirtualCardProviderNetwork.Marqeta, ProductType.Consumer).BaseUrl;

            // The card token is appended to the URL that the service creates
            var tokenLimit = _appsettings.DigitalWalletTokenCountLimit;
            string expected = $"{url}digitalwallettokens/card/{cardToken}?count={tokenLimit}";

            // Value captured in callback
            var uriTask = _mockHttpMessageHandler.MockGet()
                .ReturnsAsync(HttpStatusCode.OK, new MarqetaDigitalWalletTokensForCardResponse() { Count = 0 });

            // Act
            _ = await _marqetaService.GetDigitalWalletTokensByCardToken(cardToken);
            var actual = (await uriTask).ToString();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task GetDigitalWalletTokensByCardToken_WhenUseMockedMobileWalletResponseTrue_ThenMockedMarqetaDigitalWalletTokensForCardResponseReturned()
        {
            // Assemble
            string expectedToken = "111-444-RR-TTT";

            // Set the mocked response to be returned.
            _appsettings.UseMockedMobileWalletResponse = true;

            // Act
            var actual = await _marqetaService.GetDigitalWalletTokensByCardToken(expectedToken);

            // Assert
            Assert.AreEqual(expectedToken, actual.Data[0].CardToken);
            Assert.AreEqual(expectedToken, actual.Data[1].CardToken);
            Assert.AreEqual(expectedToken, actual.Data[2].CardToken);
        }
        #endregion GetDigitalWalletTokensByCardToken Tests

        #region PostDigitalWalletTokenTransitionAsync Test
        [Test]
        public async Task PostDigitalWalletTokenTransitionAsync_ReturnsDeserialized_MarqetaWalletTransitionResponse_OnSuccessStatusCode()
        {
            // Assemble
            var expectedResponse = new MarqetaWalletTokenTransitionResponse()
            {
                WalletTransitionToken = "TestWalletTransitionToken",
                WalletTransitionType = "state.activated.test",
                State = MarqetaDigitalWalletTokenStatus.ACTIVE,
                FulfillmentStatus = "PROVISIONED_TEST"
            };
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
                });

            // Act
            var response = await _marqetaService.PostDigitalWalletTokenTransitionAsync(new MarqetaWalletTokenTransitionRequest());

            // Assert
            Assert.AreEqual(expectedResponse.WalletTransitionType, response.WalletTransitionType);
            Assert.AreEqual(expectedResponse.State, response.State);
            Assert.AreEqual(expectedResponse.WalletTransitionType, response.WalletTransitionType);
            Assert.AreEqual(expectedResponse.FulfillmentStatus, response.FulfillmentStatus);
        }

        [Test]
        public async Task PostDigitalWalletTokenTransitionAsync_WhenUseMockedMobileWalletResponseTrue_ThenMockedMarqetaWalletTokenTransitionResponseReturned()
        {
            // Assemble
            var request = new MarqetaWalletTokenTransitionRequest()
            {
                State = MarqetaDigitalWalletTokenStatus.ACTIVE,
                DigitalWalletToken = new MarqetaDigitalWalletTokenForTransition
                {
                    Token = "MyTokenTotransition"
                }
            };

            // Set the mocked response to be returned.
            _appsettings.UseMockedMobileWalletResponse = true;

            // Act
            var actual = await _marqetaService.PostDigitalWalletTokenTransitionAsync(request);

            // Assert
            Assert.AreEqual(request.State, actual.State);
        }

        [Test]
        public void PostDigitalWalletTokenTransitionAsync_ThrowsAnHttpException_OnUnsuccessfulStatusCode()
        {
            // Assemble
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonSerializer.Serialize(new {}))
                });

            // Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _marqetaService.PostDigitalWalletTokenTransitionAsync(new MarqetaWalletTokenTransitionRequest()));
        }
        #endregion PostDigitalWalletTokenTransitionAsync Test

        #region PutUserAsync Tests
        [Test]
        public async Task PutUserAsync_WhenUserTokenPassedIn_ThenTokenInjectedIntoRequestUri()
        {
            // Assemble
            string userToken = "1234567";
            string url = _appsettings.GetCardProviderSetting(VirtualCardProviderNetwork.Marqeta, ProductType.Consumer).BaseUrl;

            // The card token is appended to the URL that the service creates 
            string expected = $"{url}users/{userToken}";

            // Value captured in callback
            string actual = string.Empty;

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>(
                    (httpRequestMessage, cancellationToken) =>
                    {
                        // Capture the URL that was called
                        actual = httpRequestMessage.RequestUri.ToString();
                    })
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new MarqetaUserResponse()))
                });

            // Act
            _ = await _marqetaService.PutUserAsync(userToken, new MarqetaUserPutRequest());

            // Assert
            Assert.AreEqual(expected, actual);
        }
        #endregion PutUserAsync Tests

        #region PostUserAsync Tests
        [Test]
        public async Task PostUserAsync_WhenUserCreated_ThenUserResponseReturned()
        {
            // Assemble
            var request = new MarqetaUserPostRequest() { Token = "123456" };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = new StringContent(JsonSerializer.Serialize(new MarqetaUserResponse() { Token = request.Token }))
                });

            // Act
            var response = await _marqetaService.PostUserAsync(request);

            // Assert
            Assert.AreEqual(request.Token, response.Token);
        }
        #endregion PostUserAsync Tests

        #region PostCardAsync Tests
        [Test]
        public async Task PostCardAsync_WhenCardCreated_ThenCardResponseReturned()
        {
            // Assemble
            var request = new MarqetaCardRequest() { Token = "123456" };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = new StringContent(JsonSerializer.Serialize(new MarqetaCardResponse() { Token = request.Token }))
                });

            // Act
            var response = await _marqetaService.PostCardAsync(request);

            // Assert
            Assert.AreEqual(request.Token, response.Token);
        }
        #endregion PostCardAsync Tests 

        #region PostPinControlTokenAsync Tests
        [Test]
        public async Task PostPinControlTokenAsync_WhenRequestTokenTypePassedIn_ThenResponseReturned()
        {
            // Arrange
            var request = new MarqetaPinControlTokenRequest { CardToken = "UniqueCardToken", ControlTokenType = "SET_PIN" };
            var expected = new MarqetaPinControlTokenResponse { ControlToken = "ControlTokenValue" };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Created,
                    Content = new StringContent(JsonSerializer.Serialize(expected))
                });

            // Act
            var actual = await _marqetaService.PostPinControlTokenAsync(request);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
        #endregion PostPinControlTokenAsync Tests

        #region PutPinAsync Tests
        [Test]
        public void PutPinAsync_WhenRequestTokenTypePassedIn_ThenResponseReturned()
        {
            // Arrange
            var request = new MarqetaPinRequest { ControlToken = "ControlTokenValue", Pin = "1234" };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NoContent
                });

            // Act
            AsyncTestDelegate testDelegate = async () =>
            {
                await _marqetaService.PutPinAsync(request);
            };

            // Assert
            Assert.DoesNotThrowAsync(async () => await testDelegate());
        }
        #endregion PutPinAsync Tests

        #region GetUserAsync Tests
        [Test]
        public async Task GetUserAsync_WhenUserLocated_ThenUserResponseReturned()
        {
            // Assemble
            var userToken = "123456";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new MarqetaUserResponse() { Token = userToken }))
                });

            // Act
            var response = await _marqetaService.GetUserAsync(userToken);

            // Assert
            Assert.AreEqual(userToken, response.Token);

        }
        #endregion GetUserAsync Tests
    }
}