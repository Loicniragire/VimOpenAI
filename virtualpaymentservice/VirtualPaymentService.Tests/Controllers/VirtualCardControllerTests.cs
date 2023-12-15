using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Controllers;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Tests.Controllers
{
    [TestFixture]
    public class VirtualCardControllerTests
    {
        #region Field
        private TelemetryClient _telemetryClient;
        private Mock<ILogger<VirtualCardController>> _mockLogger;
        private Mock<IVirtualCardProvider> _mockVirtualCardProvider;
        private Mock<IVPayTransactionProvider> _mockVPayTransactionProvider;
        private VirtualCardController _controller;
        private Mock<HttpRequest> _mockHttpRequest;

        #endregion Field

        #region Property
        private string ExpectedRequestMethodWithRoute => $"{_mockHttpRequest.Object.Method} - {_mockHttpRequest.Object.Path.Value}";
        #endregion Property

        #region Setup
        [SetUp]
        public void SetUp()
        {
            _telemetryClient = new TelemetryClient(default(TelemetryConfiguration));
            _mockLogger = new Mock<ILogger<VirtualCardController>>();
            _mockVirtualCardProvider = new Mock<IVirtualCardProvider>();
            _mockVPayTransactionProvider = new Mock<IVPayTransactionProvider>();
            _controller = new VirtualCardController(_telemetryClient, _mockLogger.Object, _mockVirtualCardProvider.Object);

            // Http method stuff
            _mockHttpRequest = new Mock<HttpRequest>();
            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _controller.ControllerContext.HttpContext = context.Object;
        }

        private void SetRequestMethodAndRoute(string method, string route)
        {
            var cleanRoute = route.Trim('/');
            _mockHttpRequest.SetupGet(x => x.Method).Returns(method);
            _mockHttpRequest.SetupGet(x => x.Path).Returns(new PathString($"/{cleanRoute}"));
        }
        #endregion Setup

        #region TokenizeApplePayVCardAsync Test
        [Test]
        public async Task PostTokenizeApplePayVCardAsync_Returns200AndAppropiateResponse_WhenSuccess()
        {
            // Assemble
            var expected = new ApplePayTokenizationResponse { ActivationData = "Active" };
            _mockVirtualCardProvider
                .Setup(p => p.GetApplePayTokenizationDataAsync(It.IsAny<ApplePayTokenizationRequest>()))
                .ReturnsAsync(expected);

            // Act
            var response = (ObjectResult)await _controller.PostTokenizeApplePayVCardAsync(new ApplePayTokenizationRequest());

            // Assert
            Assert.AreEqual(expected, response.Value);
            Assert.AreEqual(StatusCodes.Status201Created, response.StatusCode);
        }

        [Test]
        public async Task PostTokenizeApplePayVCardAsync_Returns500AndProblemResponse_WhenUnsuccessful()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetApplePayTokenizationDataAsync(It.IsAny<ApplePayTokenizationRequest>()))
                .ReturnsAsync(new ApplePayTokenizationResponse());

            // Act
            var response = (ObjectResult)await _controller.PostTokenizeApplePayVCardAsync(new ApplePayTokenizationRequest());

            // Assert
            Assert.IsTrue(response.Value is ProblemDetails);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        [Test]
        public async Task TokenizeApplePayVCardAsync_Returns500AndProblemResponse_WhenException()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetApplePayTokenizationDataAsync(It.IsAny<ApplePayTokenizationRequest>()))
                .ThrowsAsync(new Exception());

            // Act
            var response = (ObjectResult)await _controller.PostTokenizeApplePayVCardAsync(new ApplePayTokenizationRequest());

            // Assert
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.IsTrue(response.Value is ProblemDetails);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
        }
        #endregion TokenizeApplePayVCardAsync Test

        #region Google Pay Tokenization Tests
        [Test]
        public async Task PostTokenizeGooglePayVCardAsync_Returns201AndAppropiateResponse_WhenSuccess()
        {
            // Assemble
            var expected = new GooglePayTokenizationResponse
            {
                CardToken = "Token",
                PushTokenizeRequestData = new GooglePayTokenizationData
                {
                    OpaquePaymentCard = "eyJraWQiOiIxVjMwT1ZCUTNUMjRZMVFBVFRRUza",
                    UserAddress = new GooglePayUserAddress
                    {
                        PostalCode = "01243"
                    }
                }
            };

            _mockVirtualCardProvider
                .Setup(p => p.GetGooglePayTokenizationDataAsync(It.IsAny<GooglePayTokenizationRequest>()))
                .ReturnsAsync(expected);

            // Act
            var response = (ObjectResult)await _controller.PostTokenizeGooglePayVCardAsync(new GooglePayTokenizationRequest());

            // Assert
            Assert.AreEqual(expected, response.Value);
            Assert.AreEqual(StatusCodes.Status201Created, response.StatusCode);
        }

        [Test]
        public async Task PostTokenizeGooglePayVCardAsync_Returns500AndProblemResponse_WhenUnsuccessful()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetGooglePayTokenizationDataAsync(It.IsAny<GooglePayTokenizationRequest>()))
                .ReturnsAsync(new GooglePayTokenizationResponse());

            // Act
            var response = (ObjectResult)await _controller.PostTokenizeGooglePayVCardAsync(new GooglePayTokenizationRequest());

            // Assert
            Assert.IsTrue(response.Value is ProblemDetails);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
        }

        [Test]
        public async Task PostTokenizeGooglePayVCardAsync_Returns500AndProblemResponse_WhenException()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetGooglePayTokenizationDataAsync(It.IsAny<GooglePayTokenizationRequest>()))
                .ThrowsAsync(new Exception());

            // Act
            var response = (ObjectResult)await _controller.PostTokenizeGooglePayVCardAsync(new GooglePayTokenizationRequest());

            // Assert
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.IsTrue(response.Value is ProblemDetails);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
        }
        #endregion Google Pay Tokenization Tests

        #region GetVCardByLeaseId Test
        [Test]
        public async Task GetVCardByLeaseId_Returns200IfVCardIsNotNull()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetVCardsByLeaseAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<VCard>());

            // Act
            var response = (ObjectResult)await _controller.GetVCardsByLeaseAsync(123);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
        }

        [Test]
        public async Task GetVCardByLeaseId_Returns500WhenException()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetVCardsByLeaseAsync(It.IsAny<long>()))
                .ThrowsAsync(new Exception());

            // Act
            var response = (ObjectResult)await _controller.GetVCardsByLeaseAsync(123);

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
        }
        #endregion GetVCardByLeaseId Test

        #region GetDigitalWalletTokensByVCardAsync Tests
        [Test]
        public async Task GetDigitalWalletTokensByVCardAsync_WhenSuccessfulReturnFromProvider_ThenReturnsHttp200()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetDigitalWalletTokensByVCardAsync(It.IsAny<string>()))
                .ReturnsAsync(new DigitalWalletTokenResponse());

            // Act
            var actual = (ObjectResult)await _controller.GetDigitalWalletTokensByVCardAsync("RandomCardToken");

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, actual.StatusCode);
        }

        [Test]
        public async Task GetDigitalWalletTokensByVCardAsync_WhenSuccessfulReturnFromProvider_ThenReturnsDigitalWalletTokenResponse()
        {
            // Assemble
            var expected = JsonSerializer.Serialize(new DigitalWalletTokenResponse());

            _mockVirtualCardProvider
                .Setup(p => p.GetDigitalWalletTokensByVCardAsync(It.IsAny<string>()))
                .ReturnsAsync(new DigitalWalletTokenResponse());

            // Act
            var actual = (ObjectResult)await _controller.GetDigitalWalletTokensByVCardAsync("RandomCardToken");

            // Assert
            Assert.AreEqual(expected, JsonSerializer.Serialize(actual.Value));
        }

        /// <summary>
        /// Validates if we get an error when calling Digital wallet provider that we return
        /// the status code we got from calling their API.
        /// </summary>
        [TestCase(StatusCodes.Status500InternalServerError, StatusCodes.Status500InternalServerError)]
        [TestCase(StatusCodes.Status502BadGateway, StatusCodes.Status502BadGateway)]
        [TestCase(StatusCodes.Status401Unauthorized, StatusCodes.Status401Unauthorized)]
        [TestCase(StatusCodes.Status408RequestTimeout, StatusCodes.Status408RequestTimeout)]
        [TestCase(StatusCodes.Status404NotFound, StatusCodes.Status404NotFound)]
        public async Task GetDigitalWalletTokensByVCardAsync_WhenHttpExceptionReturnFromProvider_ThenReturnsHttpStatusCodeFromProvider(int errorReturnedFromProvider, int expected)
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetDigitalWalletTokensByVCardAsync(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("HTTP error from provider.", null, (HttpStatusCode)errorReturnedFromProvider));

            // Act
            var actual = (ObjectResult)await _controller.GetDigitalWalletTokensByVCardAsync("RandomCardToken");

            // Assert
            Assert.AreEqual(expected, actual.StatusCode);
        }

        /// <summary>
        /// Validate not found errors returned from Digital wallet provider return message that card token was not found on network.
        ///     - 404 is returned when the Digital wallet provider API does not locate a card using the token passed in.
        /// Validate all other errors return generic message to caller since we do not know why the error was thrown.
        /// </summary>
        [TestCase(StatusCodes.Status500InternalServerError, "An error occurred while locating digital wallet tokens for cardToken value 111-444 passed in.", "111-444")]
        [TestCase(StatusCodes.Status502BadGateway, "An error occurred while locating digital wallet tokens for cardToken value 111-444 passed in.", "111-444")]
        [TestCase(StatusCodes.Status401Unauthorized, "An error occurred while locating digital wallet tokens for cardToken value 111-444 passed in.", "111-444")]
        [TestCase(StatusCodes.Status408RequestTimeout, "An error occurred while locating digital wallet tokens for cardToken value 111-444 passed in.", "111-444")]
        [TestCase(StatusCodes.Status404NotFound, "The cardToken value 111-444 passed in was not located on the virtual card provider network.", "111-444")]
        public async Task GetDigitalWalletTokensByVCardAsync_WhenHttpExceptionReturnFromProvider_ThenReturnErrorMessageBasedOnStatusCode(int errorReturnedFromProvider, string expected, string cardToken)
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetDigitalWalletTokensByVCardAsync(It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("HTTP error from provider.", null, (HttpStatusCode)errorReturnedFromProvider));

            // Act
            var actual = (ObjectResult)await _controller.GetDigitalWalletTokensByVCardAsync(cardToken);

            // Assert
            Assert.AreEqual(expected, ((ProblemDetails)actual.Value).Title);
        }

        [Test]
        public async Task GetDigitalWalletTokensByVCardAsync_WhenGeneralExceptionReturnedFromProvider_ThenReturnsHttp500()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetDigitalWalletTokensByVCardAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            // Act
            var actual = (ObjectResult)await _controller.GetDigitalWalletTokensByVCardAsync("RandomCardToken");

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actual.StatusCode);
        }


        #endregion GetDigitalWalletTokensByVCardAsync Tests

        #region PostWalletTokenTransitionAsync Tests

        [Test]
        public async Task PostWalletTokenTransitionAsync_Returns201AndAppropiateResponse_WhenSuccess()
        {
            // Assemble
            var expected = new DigitalWalletTokenTransitionResponse() { WalletTransitionToken = "0123456789-abcdef" };
            _mockVirtualCardProvider
                .Setup(p => p.TransitionDigitalWalletTokenAsync(It.IsAny<DigitalWalletTokenTransitionRequest>()))
                .ReturnsAsync(expected);

            // Act
            var response = (ObjectResult)await _controller.PostWalletTokenTransitionAsync(new DigitalWalletTokenTransitionRequest());

            // Assert
            Assert.AreEqual(expected, response.Value);
            Assert.AreEqual(StatusCodes.Status201Created, response.StatusCode);
        }

        [TestCase(StatusCodes.Status400BadRequest, "An exception occurred while attempting to transition the following WalletToken: 4321, for the associated CardToken: 1234.")]
        [TestCase(StatusCodes.Status404NotFound, "Virtual card provider could not find the WalletToken: 4321 or WalletTransitionReasonCode: 00, for the associated CardToken: 1234.")]
        [TestCase(StatusCodes.Status500InternalServerError, "An exception occurred while attempting to transition the following WalletToken: 4321, for the associated CardToken: 1234.")]
        public async Task PostWalletTokenTransitionAsync_ReturnsCorrectStatusCode_Message_And_ProblemResponse_Upon_Catching_HttpRequestException(int httpExceptionStatusCode, string expectedMsg)
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.TransitionDigitalWalletTokenAsync(It.IsAny<DigitalWalletTokenTransitionRequest>()))
                .ThrowsAsync(new HttpRequestException("HttpRequestException message.", null, (HttpStatusCode)httpExceptionStatusCode));

            // Act
            var response = (ObjectResult)await _controller.PostWalletTokenTransitionAsync(
                new DigitalWalletTokenTransitionRequest
                {
                    DigitalWalletToken = new DigitalWalletToken { CardToken = "1234", WalletToken = "4321" },
                    WalletTransitionReasonCode = "00"
                });

            // Assert
            Assert.IsTrue(response.Value is ProblemDetails);
            Assert.AreEqual(expectedMsg, ((ProblemDetails)response.Value).Title);
            Assert.AreEqual(httpExceptionStatusCode, response.StatusCode);
        }

        /// <summary>
        /// Note: If these tests break and must be updated due to changes in Messages,
        /// please ensure any alert checking for the related logging messages are updated to capture the new message.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PostWalletTokenTransitionAsync_Returns500AndProblemResponse_When_General_Exception_Caught()
        {
            // Assemble
            var expectedException = new Exception("ExceptionMessage");

            _mockVirtualCardProvider
                .Setup(p => p.TransitionDigitalWalletTokenAsync(It.IsAny<DigitalWalletTokenTransitionRequest>()))
                .ThrowsAsync(new Exception(expectedException.Message));

            // Act
            var response = (ObjectResult)await _controller.PostWalletTokenTransitionAsync(
                new DigitalWalletTokenTransitionRequest
                {
                    DigitalWalletToken = new DigitalWalletToken { CardToken = "1234", WalletToken = "4321" }
                });

            // Assert
            Assert.IsTrue(response.Value is ProblemDetails);
            Assert.AreEqual(
                $"PostWalletTokenTransitionAsync caught an exception - {expectedException.Message}, while processing request for WalletToken: 4321, CardToken: 1234.",
                ((ProblemDetails)response.Value).Title);

            _mockLogger.Verify(
                logger => logger.Log(LogLevel.Error,
                    It.Is<string>(s =>
                        s.Equals($"PostWalletTokenTransitionAsync caught an exception - {expectedException.Message}.")),
                    It.Is<Exception>(e => e.Message == expectedException.Message),
                    It.Is<IDictionary<string, object>>(d =>
                        d["CardToken"].Equals("1234") && d["WalletToken"].Equals("4321"))), Times.Once);

        }

        /// <summary>
        /// Note: If these tests break and must be updated due to changes in Messages,
        /// please ensure any alert checking for the related logging messages are updated to capture the new message.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PostWalletTokenTransitionAsync_Returns500AndProblemResponse_When_NullRequest_And_General_Exception_Caught()
        {
            // Assemble
            var expectedException = new Exception("ExceptionMessage");

            _mockVirtualCardProvider
                .Setup(p => p.TransitionDigitalWalletTokenAsync(It.IsAny<DigitalWalletTokenTransitionRequest>()))
                .ThrowsAsync(new Exception(expectedException.Message));

            // Act
            var response = (ObjectResult)await _controller.PostWalletTokenTransitionAsync(null);

            // Assert
            Assert.IsTrue(response.Value is ProblemDetails);
            Assert.AreEqual(
                $"PostWalletTokenTransitionAsync caught an exception - {expectedException.Message}, while processing request for WalletToken: , CardToken: .",
                ((ProblemDetails)response.Value).Title);

            _mockLogger.Verify(
                logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<Exception>(),
                    It.IsAny<IDictionary<string, object>>()), Times.Once);

        }

        #endregion PostWalletTokenTransitionAsync Tests

        #region PatchCardUserAsync Tests
        [Test]
        public async Task PatchCardUserAsync_WhenUserUpdateSuccess_ThenReturnsHttp200()
        {
            // Assemble
            var expectedBody = new CardUserUpdateResponse()
            {
                PhoneNumber = "+18015551212",
                Active = true,
                CreatedTime = DateTime.Now,
                LastModifiedTime = DateTime.Now
            };

            var userToken = "1234567";

            _mockVirtualCardProvider
                .Setup(p => p.UpdateCardUserAsync(It.IsAny<string>(), It.IsAny<CardUserUpdateRequest>()))
                .ReturnsAsync(expectedBody);

            // Act
            var response = (ObjectResult)await _controller.PatchCardUserAsync(userToken, new CardUserUpdateRequest());

            // Assert
            Assert.AreEqual(expectedBody, response.Value);
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
        }

        /// <summary>
        /// Validates if we get an error when calling Digital wallet provider that we return
        /// the status code we got from calling their API.
        /// </summary>
        [TestCase(StatusCodes.Status500InternalServerError, StatusCodes.Status500InternalServerError)]
        [TestCase(StatusCodes.Status502BadGateway, StatusCodes.Status502BadGateway)]
        [TestCase(StatusCodes.Status401Unauthorized, StatusCodes.Status401Unauthorized)]
        [TestCase(StatusCodes.Status408RequestTimeout, StatusCodes.Status408RequestTimeout)]
        [TestCase(StatusCodes.Status404NotFound, StatusCodes.Status404NotFound)]
        public async Task PatchCardUserAsync_WhenHttpExceptionReturnFromProvider_ThenReturnsHttpStatusCodeFromProvider(int errorReturnedFromProvider, int expected)
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.UpdateCardUserAsync(It.IsAny<string>(), It.IsAny<CardUserUpdateRequest>()))
                .ThrowsAsync(new HttpRequestException("HTTP error from provider.", null, (HttpStatusCode)errorReturnedFromProvider));

            // Act
            var actual = (ObjectResult)await _controller.PatchCardUserAsync("Random User Token", new CardUserUpdateRequest());

            // Assert
            Assert.AreEqual(expected, actual.StatusCode);
        }

        [Test]
        public async Task PatchCardUserAsync_WhenGeneralExceptionReturnedFromProvider_ThenReturnsHttp500()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.UpdateCardUserAsync(It.IsAny<string>(), It.IsAny<CardUserUpdateRequest>()))
                .ThrowsAsync(new Exception());

            // Act
            var actual = (ObjectResult)await _controller.PatchCardUserAsync("Random User Token", new CardUserUpdateRequest());

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actual.StatusCode);
        }
        #endregion PatchCardUserAsync Tests

        #region GetCardUserAsync Tests
        [Test]
        public async Task PatchCardUserAsync_WhenUserLocatedAtProvider_Then200OkReturned()
        {
            // Assemble
            var expectedBody = new CardUserResponse();

            _mockVirtualCardProvider
                .Setup(p => p.GetCardUserAsync(It.IsAny<string>(), It.IsAny<VirtualCardProviderNetwork>(), It.IsAny<ProductType>()))
                .ReturnsAsync(expectedBody);

            // Act
            var response = (ObjectResult)await _controller.GetCardUserAsync("123456", VirtualCardProviderNetwork.Marqeta, ProductType.Commercial);

            // Assert
            Assert.AreEqual(expectedBody, response.Value);
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
        }

        [Test]
        public async Task PatchCardUserAsync_WhenProductTypeIsNull_ThenReturn400BadRequest()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetCardUserAsync(It.IsAny<string>(), It.IsAny<VirtualCardProviderNetwork>(), It.IsAny<ProductType>()))
                .ReturnsAsync(new CardUserResponse());

            // Act
            var response = (ObjectResult)await _controller.GetCardUserAsync("123456", VirtualCardProviderNetwork.Marqeta, ProductType.Null);

            // Assert
            Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [TestCase(StatusCodes.Status500InternalServerError)]
        [TestCase(StatusCodes.Status502BadGateway)]
        [TestCase(StatusCodes.Status401Unauthorized)]
        [TestCase(StatusCodes.Status408RequestTimeout)]
        [TestCase(StatusCodes.Status404NotFound)]
        public async Task PatchCardUserAsync_WhenUserSearchReturnsHttpException_ThenSameErrorCodeAndMessageReturned(int expected)
        {
            // Assemble
            var expectedTitle = "HTTP error from provider.";

            _mockVirtualCardProvider
                .Setup(p => p.GetCardUserAsync(It.IsAny<string>(), It.IsAny<VirtualCardProviderNetwork>(), It.IsAny<ProductType>()))
                .ThrowsAsync(new HttpRequestException(expectedTitle, null, (HttpStatusCode)expected));

            // Act
            var actual = (ObjectResult)await _controller.GetCardUserAsync("123456", VirtualCardProviderNetwork.Marqeta, ProductType.Commercial);
            var actualBody = (ProblemDetails)actual.Value;

            // Assert
            Assert.AreEqual(expected, actual.StatusCode);
            Assert.AreEqual(expectedTitle, actualBody.Title);
        }

        [Test]
        public async Task PatchCardUserAsync_WhenUserSearchReturnsException_Then500ServerErrorReturned()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetCardUserAsync(It.IsAny<string>(), It.IsAny<VirtualCardProviderNetwork>(), It.IsAny<ProductType>()))
                .ThrowsAsync(new Exception("Random Error!"));

            // Act
            var actual = (ObjectResult)await _controller.GetCardUserAsync("123456", VirtualCardProviderNetwork.Marqeta, ProductType.Commercial);

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actual.StatusCode);
        }
        #endregion GetCardUserAsync Tests

        #region GetProductTypeAsync Tests

        [TestCase(null, 0, 123, 123)]
        [TestCase(null, 0, null, 0)]
        [TestCase(VirtualCardProviderNetwork.Marqeta, VirtualCardProviderNetwork.Marqeta, null, 0)]
        [TestCase(VirtualCardProviderNetwork.Marqeta, VirtualCardProviderNetwork.Marqeta, 123, 123)]
        public async Task Given_GetProductTypeAsync_When_Called_Then_CallsProviderWithExpectedParameters(
            VirtualCardProviderNetwork vcpNetworkParam, VirtualCardProviderNetwork expectedVcpNetworkParam,
            int storeIdParam, int expectedStoreIdParam)
        {
            // Arrange & Assert
            _mockVirtualCardProvider.Setup(p =>
                    p.GetProductTypeAsync(It.IsAny<VirtualCardProviderNetwork>(), It.IsAny<int>()))
                .Callback<VirtualCardProviderNetwork, int>((actualVcpNetworkParam, actualStoreIdParam) =>
                {
                    Assert.AreEqual(expectedVcpNetworkParam, actualVcpNetworkParam);
                    Assert.AreEqual(expectedStoreIdParam, actualStoreIdParam);
                });

            // Act
            _ = await _controller.GetVCardProductTypeAsync(vcpNetworkParam, storeIdParam);
        }

        [Test]
        public async Task Given_GetProductTypeAsync_When_ResultsNotNull_ThenReturns200Ok()
        {
            // Arrange
            var expected = new StoreProductType
            {
                ProductTypeId = ProductType.Consumer,
                StoreId = 123,
                CustomerInfoRequired = true
            };

            _mockVirtualCardProvider
                .Setup(p => p.GetProductTypeAsync(It.IsAny<VirtualCardProviderNetwork>(), It.IsAny<int>()))
                .ReturnsAsync(expected);

            // Act
            var result =
                (ObjectResult)await _controller.GetVCardProductTypeAsync(VirtualCardProviderNetwork.Marqeta, 123);
            var actual = result.Value;

            // Assert
            Assert.IsNotNull(actual);
            actual.Should().BeEquivalentTo(expected);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [Test]
        public async Task Given_GetProductTypeAsync_When_ResultsNull_ThenReturns204NoContent()
        {
            // Arrange & Act
            var result = (NoContentResult)await _controller.GetVCardProductTypeAsync(VirtualCardProviderNetwork.Marqeta, 123);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
        }

        [Test]
        public async Task Given_GetVCardProductTypeAsync_When_Exception_Then_Returns500StatusCodeAndLogsError()
        {
            // Arrange
            var exception = new Exception();
            _mockVirtualCardProvider.Setup(p => p.GetProductTypeAsync(It.IsAny<VirtualCardProviderNetwork>(), It.IsAny<int>()))
                    .ThrowsAsync(exception);

            // Act
            var result = (ObjectResult)await _controller.GetVCardProductTypeAsync(VirtualCardProviderNetwork.Marqeta, 123);

            // Assert
            _mockLogger.Verify(l => l.Log(
                LogLevel.Error,
                It.Is<string>(m => m.StartsWith("An exception occurred while attempting to get store product type for")),
                exception,
                It.IsAny<IDictionary<string, object>>()), Times.Once);

            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }
        #endregion GetProductTypeAsync Tests

        #region GetVCardProvidersAsync Tests
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public async Task Given_GetVCardProvidersAsync_When_Called_Then_CallsProviderWithCorrectParameters(bool? param, bool expectedParam)
        {
            // Arrange & Assert
            _mockVirtualCardProvider.Setup(p => p.GetVCardProvidersAsync(It.IsAny<bool>()))
                .Callback<bool>((actualParam) =>
                {
                    Assert.AreEqual(expectedParam, actualParam);
                });

            // Act
            _ = await (param == null ?
                _controller.GetVCardProvidersAsync() :
                _controller.GetVCardProvidersAsync((bool)param));
        }

        [Test]
        public async Task Given_GetVCardProvidersAsync_When_Called_Then_ReturnsOKStatusCodeOnSuccess()
        {
            // Arrange
            var expected = new List<VCardProvider>
            {
                {
                    new VCardProvider
                    {
                        AuthExpireDays = 12,
                        IsActive = true,
                        BusinessRank = 3,
                        CreditLimit = 124.00M,
                        ProviderId = 1,
                        ProviderName = "Test Provider"
                    }
                }
            };
            _mockVirtualCardProvider.Setup(p => p.GetVCardProvidersAsync(It.IsAny<bool>()))
                .ReturnsAsync(expected);

            // Act
            var result = (ObjectResult)await _controller.GetVCardProvidersAsync();
            var actual = result.Value;

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Given_GetVCardProvidersAsync_When_Exception_Then_Returns500StatusCodeAndLogsError()
        {
            // Arrange
            var exception = new Exception();
            _mockVirtualCardProvider.Setup(p => p.GetVCardProvidersAsync(It.IsAny<bool>()))
                    .ThrowsAsync(exception);

            // Act
            var result = (ObjectResult)await _controller.GetVCardProvidersAsync();

            // Assert
            _mockLogger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<string>(),
                exception,
                It.IsAny<IDictionary<string, object>>()), Times.Once);

            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }
        #endregion GetVCardProvidersAsync Tests

        #region GetWalletBatchConfigAsync Tests
        [Test]
        public async Task GetWalletBatchConfigAsync_WhenNoExceptionThrown_ThenRespondsWith200AndDateTime()
        {
            // Assemble
            var expectedDate = new DateTime();
            var expected = new WalletBatchConfigResponse()
            {
                LastTerminationProcessedDate = expectedDate
            };

            _mockVirtualCardProvider
                .Setup(p => p.GetWalletBatchConfigAsync())
                .ReturnsAsync(expected);

            // Act
            var result = (ObjectResult)await _controller.GetWalletBatchConfigAsync();
            var actual = result.Value;

            // Assert
            actual.Should().BeEquivalentTo(expected);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [Test]
        public async Task GetWalletBatchConfigAsync_WhenExceptionOccurs_ThenWith500StatusCode()
        {
            // Assemble
            _mockVirtualCardProvider
                .Setup(p => p.GetWalletBatchConfigAsync())
                .ThrowsAsync(new Exception());

            // Act
            var result = (ObjectResult)await _controller.GetWalletBatchConfigAsync();

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }
        #endregion GetWalletBatchConfigAsync Tests

        #region PutWalletBatchConfigAsync Tests
        [Test]
        public async Task PutWalletBatchConfigAsync_WhenUpdateSuccessful_ThenRespondsWith201StatusCode()
        {
            // Assemble
            var request = new UpdateWalletBatchConfigRequest
            {
                LastTerminationProcessedDate = DateTimeOffset.Now,
                UpdatedBy = null
            };

            _mockVirtualCardProvider
                .Setup(p => p.UpdateWalletBatchConfigAsync(It.IsAny<UpdateWalletBatchConfigRequest>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = (StatusCodeResult)await _controller.PutWalletBatchConfigAsync(request);

            // Assert
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        }

        [Test]
        public async Task PutWalletBatchConfigAsync_WhenExceptionIsThrown_ThenRespondsWith500StatusCode()
        {
            // Assemble
            var request = new UpdateWalletBatchConfigRequest
            {
                LastTerminationProcessedDate = DateTimeOffset.Now,
                UpdatedBy = null
            };

            _mockVirtualCardProvider
                .Setup(p => p.UpdateWalletBatchConfigAsync(It.IsAny<UpdateWalletBatchConfigRequest>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = (ObjectResult)await _controller.PutWalletBatchConfigAsync(request);

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }
        #endregion GetWalletBatchConfigAsync Tests

        #region GetCancelledVCardsByDateTimeAsync Tests
        [Test]
        public async Task GetCancelledVCardsByDateTimeAsync_WhenGetSuccessful_ThenRespondsWith200StatusCode_AndLogsCount()
        {
            // Assemble
            var request = DateTimeOffset.Now;
            var expected = new List<CancelledVCard>
            {
                new CancelledVCard
                {
                    LeaseId = 123456,
                    VCardId = 6123,
                    VCardProviderId = 6,
                    ReferenceId = "some reference"
                }
            };

            _mockVirtualCardProvider
                .Setup(p => p.GetCancelledVCardsByDateTimeAsync(request))
                .ReturnsAsync(expected);

            // Act
            var result = (ObjectResult)await _controller.GetCancelledVCardsByDateTimeAsync(request);

            // Assert
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Info,
                It.Is<string>(str => str.Contains($"Found {expected.Count} cancelled vcard(s)")),
                It.IsAny<Exception>(),
                It.IsAny<IDictionary<string, object>>()),
            Times.Once);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            result.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetCancelledVCardsByDateTimeAsync_WhenExceptionIsThrown_ThenRespondsWith500StatusCode()
        {
            // Assemble
            var request = DateTimeOffset.Now;
            var errorMessage = $"An exception occurred while retrieving vcards updated on or after {request.LocalDateTime}.";

            _mockVirtualCardProvider
                .Setup(p => p.GetCancelledVCardsByDateTimeAsync(request))
                .ThrowsAsync(new Exception());

            // Act
            var result = (ObjectResult)await _controller.GetCancelledVCardsByDateTimeAsync(request);

            // Assert
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.Is<string>(str => str.Contains(errorMessage)),
                It.IsAny<Exception>(),
                It.IsAny<IDictionary<string, object>>()),
            Times.Once);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }
        #endregion GetWalletBatchConfigAsync Tests

        #region PostCreateCardAsync Tests
        [Test]
        public async Task PostCreateCardAsync_WhenHttpErrorEncountered_ThenSameErrorCodeReturnedToCaller()
        {
            // Assemble
            var request = new VirtualCardRequest() { LeaseId = 123456 };

            var expectedResponseCode = StatusCodes.Status201Created;
            var expectedBody = new VirtualCardResponse() { LeaseId = (int)request.LeaseId };

            _mockVirtualCardProvider
                .Setup(p => p.CreateCardAsync(request))
                .ReturnsAsync(expectedBody);

            _mockVirtualCardProvider
                .Setup(p => p.IsRequiredUserPhoneMissingAsync(request))
                .ReturnsAsync(false);

            // Act
            var result = (ObjectResult)await _controller.PostCreateCardAsync(request);

            // Assert
            Assert.AreEqual(expectedResponseCode, result.StatusCode);
            result.Value.Should().BeEquivalentTo(expectedBody);
        }

        [TestCase(StatusCodes.Status500InternalServerError, StatusCodes.Status500InternalServerError)]
        [TestCase(StatusCodes.Status502BadGateway, StatusCodes.Status502BadGateway)]
        [TestCase(StatusCodes.Status401Unauthorized, StatusCodes.Status401Unauthorized)]
        [TestCase(StatusCodes.Status408RequestTimeout, StatusCodes.Status408RequestTimeout)]
        [TestCase(StatusCodes.Status404NotFound, StatusCodes.Status404NotFound)]
        public async Task PostCreateCardAsync_WhenHttpExceptionReturnFromProvider_ThenReturnsHttpStatusCodeFromProvider(int errorReturnedFromProvider, int expected)
        {
            // Assemble
            var request = new VirtualCardRequest() { LeaseId = 123456 };

            _mockVirtualCardProvider
                .Setup(p => p.CreateCardAsync(request))
                .ThrowsAsync(new HttpRequestException("HTTP error from provider.", null, (HttpStatusCode)errorReturnedFromProvider));

            _mockVirtualCardProvider
                .Setup(p => p.IsRequiredUserPhoneMissingAsync(request))
                .ReturnsAsync(false);

            // Act
            var actual = (ObjectResult)await _controller.PostCreateCardAsync(request);

            // Assert
            Assert.AreEqual(expected, actual.StatusCode);
        }

        [Test]
        public async Task PostCreateCardAsync_WhenNonHttpExceptionReturnFromProvider_ThenReturnsGeneric500()
        {
            // Assemble
            var request = new VirtualCardRequest() { LeaseId = 123456 };

            _mockVirtualCardProvider
                .Setup(p => p.CreateCardAsync(request))
                .ThrowsAsync(new Exception("Random non-HTTP error."));

            _mockVirtualCardProvider
                .Setup(p => p.IsRequiredUserPhoneMissingAsync(request))
                .ReturnsAsync(false);

            // Act
            var actual = (ObjectResult)await _controller.PostCreateCardAsync(request);

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actual.StatusCode);
        }

        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("        ", true)]
        [TestCase(null, false)]
        public async Task PostCreateCardAsync_WhenUserPhoneIsRequiredAndMissing_ThenReturnsHttp400BadRequest(string phoneNumber, bool userPresent)
        {
            // Assemble
            var user = new CardUserUpdateRequest { PhoneNumber = phoneNumber, PhoneNumberCountryCode = "1" };
            var request = new VirtualCardRequest() { LeaseId = 123456, User = userPresent ? user : null };

            _mockVirtualCardProvider
                .Setup(p => p.IsRequiredUserPhoneMissingAsync(request))
                .ReturnsAsync(true);

            // Act
            var actual = (ObjectResult)await _controller.PostCreateCardAsync(request);

            // Assert
            Assert.AreEqual(StatusCodes.Status400BadRequest, actual.StatusCode);
        }
        #endregion PostCreateCardAsync Tests

        #region PostCancelVCardAsync Tests
        [TestCase(true, StatusCodes.Status201Created, TestName = "When VCard canceled, responds 201 status code")]
        [TestCase(false, StatusCodes.Status500InternalServerError, TestName = "When internal error, responds 500 status code")]
        public async Task Given_PostCancelVCardAsync_When_CancelVCardsAsyncDoesNotThrowException_Then_CorrectHttpResponseCodeSent(bool cancelResult, int expectedStatus)
        {
            // Arrange
            var request = new CancelVCardRequest();

            _mockVirtualCardProvider
                .Setup(p => p.CancelVCardAsync(request))
                .ReturnsAsync(cancelResult);

            // Act
            var response = (ObjectResult)await _controller.PostCancelVCardAsync(request);
            var actualStatus = response.StatusCode;

            // Assert
            Assert.AreEqual(expectedStatus, actualStatus);
        }

        [Test]
        public async Task Given_PostCancelVCardAsync_When_CancelVCardsAsyncThrowsException_Then_ExceptionIsLoggedAnd500StatusCode()
        {
            // Arrange
            _mockVirtualCardProvider
                .Setup(p => p.CancelVCardAsync(It.IsAny<CancelVCardRequest>()))
                .ThrowsAsync(new Exception("Whoops!"));

            // Act
            var response = (ObjectResult)await _controller.PostCancelVCardAsync(new CancelVCardRequest());
            var statusCode = response.StatusCode;

            // Assert
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<string>(),
                    It.IsAny<Exception>(),
                    It.IsAny<IDictionary<string, object>>()),
                Times.Once);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCode);
        }
        #endregion PostCancelVCardAsync Tests

        #region GetVirtualCardAuthorizationsAsync Tests
        [Test]
        public async Task GetVirtualCardAuthorizationsAsync_WhenAuthorizationsLocated_ThenOKResponseReturned()
        {
            // Arrange 
            var vCardId = 12345;
            var expected = new GetVirtualCardAuthorizationsResponse();

            _mockVPayTransactionProvider
                .Setup(p => p.GetVirtualCardAuthorizationsAsync(It.IsAny<int>()))
                .ReturnsAsync(expected);

            // Act
            var actual = (ObjectResult)await _controller.GetVirtualCardAuthorizationsAsync(vCardId, _mockVPayTransactionProvider.Object);

            // Assert
            actual.StatusCode.Should().Be(StatusCodes.Status200OK);
            actual.Value.Should().BeEquivalentTo(expected);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Info,
                    ExpectedRequestMethodWithRoute,
                    null,
                    new Dictionary<string, object> { { nameof(vCardId), vCardId }, }),
                Times.Once);
        }

        [Test]
        public async Task GetVirtualCardAuthorizationsAsync_WhenUnexpectedErrorEncountered_ThenInternalServerErrorReturned()
        {
            // Arrange 
            var vCardId = 12345;
            var exception = new Exception("Random error encountered!");
            var expected = new ProblemDetails()
            {
                Title = $"An unknown exception occurred while attempting to retrieve saved authorization records for {nameof(vCardId)} {vCardId}.",
                Detail = "See logged errors for more details.",
                Status = StatusCodes.Status500InternalServerError
            };

            _mockVPayTransactionProvider
                .Setup(p => p.GetVirtualCardAuthorizationsAsync(It.IsAny<int>()))
                .ThrowsAsync(exception);

            // Act
            var actual = (ObjectResult)await _controller.GetVirtualCardAuthorizationsAsync(vCardId, _mockVPayTransactionProvider.Object);

            // Assert
            actual.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            actual.Value.Should().BeEquivalentTo(expected);
            _mockLogger.Verify(l => l.Log(LogLevel.Error,
                                          $"An exception occurred while retrieving saved authorization records for {nameof(vCardId)} {vCardId}!",
                                          exception,
                                          null),
                                    Times.Once);
        }
        #endregion GetVirtualCardAuthorizationsAsync Tests

        #region PostVCardAuthorizationAsync Tests
        [Test]
        public async Task Given_PostVCardAuthorizationAsync_When_NotDuplicate_Then_CorrectResponseWith201StatusCodeIsReturned()
        {
            // Arrange
            SetRequestMethodAndRoute("POST", "/vcard/authorization");
            var request = new VCardPurchaseAuthRequest
            {
                ReferenceId = Guid.NewGuid().ToString()
            };

            var loggedBody = JsonSerializer.Serialize(request);

            var expectedResponse = new VCardPurchaseAuthResponse
            {
                ReferenceId = request.ReferenceId
            };

            var result = new AddVCardAuthorizationResult(expectedResponse, false);

            _mockVPayTransactionProvider
                .Setup(p => p.AddVCardAuthorizationAsync(It.IsAny<VCardPurchaseAuthRequest>()))
                .ReturnsAsync(result);

            // Act
            var response = (ObjectResult)await _controller.PostVCardAuthorizationAsync(request, _mockVPayTransactionProvider.Object);
            var actualResponse = response.Value;
            var actualStatusCode = response.StatusCode;

            // Assert
            Assert.AreEqual(expectedResponse, actualResponse);
            Assert.AreEqual(StatusCodes.Status201Created, actualStatusCode);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Info,
                    ExpectedRequestMethodWithRoute,
                    null,
                    new Dictionary<string, object> { { nameof(VCardPurchaseAuthRequest), loggedBody }, }),
                Times.Once);
        }

        [Test]
        public async Task Given_PostVCardAuthorizationAsync_When_IsDuplicate_Then_204StatusCodeIsReturned()
        {
            // Arrange
            var request = new VCardPurchaseAuthRequest
            {
                ReferenceId = Guid.NewGuid().ToString()
            };

            var result = new AddVCardAuthorizationResult(null, true);

            _mockVPayTransactionProvider
                .Setup(p => p.AddVCardAuthorizationAsync(It.IsAny<VCardPurchaseAuthRequest>()))
                .ReturnsAsync(result);

            // Act
            var response = (StatusCodeResult)await _controller.PostVCardAuthorizationAsync(request, _mockVPayTransactionProvider.Object);
            var actualStatusCode = response.StatusCode;

            // Assert
            Assert.AreEqual(StatusCodes.Status204NoContent, actualStatusCode);
        }

        [Test]
        public async Task Given_PostVCardAuthorizationAsync_When_Exception_Then_LogAndReturn500()
        {
            // Arrange
            var request = new VCardPurchaseAuthRequest
            {
                ReferenceId = Guid.NewGuid().ToString()
            };

            var ex = new Exception("Uh oh!");

            _mockVPayTransactionProvider
                .Setup(p => p.AddVCardAuthorizationAsync(It.IsAny<VCardPurchaseAuthRequest>()))
                .ThrowsAsync(ex);

            // Act
            var response = (ObjectResult)await _controller.PostVCardAuthorizationAsync(request, _mockVPayTransactionProvider.Object);

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
            _mockLogger.Verify(l => l.Log(LogLevel.Error,
                                          It.Is<string>(s => s.Contains(request.ReferenceId)),
                                          ex,
                                          It.IsAny<IDictionary<string, object>>()),
                               Times.Once);
        }
        #endregion PostVCardAuthorizationAsync Tests

        #region GetSettlementTransactionsAsync Tests
        [Test]
        public async Task GetSettlementTransactionsAsync_WhenSettlementsLocated_ThenOKResponseReturned()
        {
            // Arrange 
            var leaseId = 1234578;
            var expected = new GetSettlementTransactionsResponse();

            _mockVPayTransactionProvider
                .Setup(p => p.GetSettlementTransactionsAsync(It.IsAny<int>()))
                .ReturnsAsync(expected);

            // Act
            var actual = (ObjectResult)await _controller.GetSettlementTransactionsAsync(leaseId, _mockVPayTransactionProvider.Object);

            // Assert
            actual.StatusCode.Should().Be(StatusCodes.Status200OK);
            actual.Value.Should().BeEquivalentTo(expected);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Info,
                    ExpectedRequestMethodWithRoute,
                    null,
                    new Dictionary<string, object> { { nameof(leaseId), leaseId }, }),
                Times.Once);
        }

        [Test]
        public async Task GetSettlementTransactionsAsync_WhenUnexpectedErrorEncountered_ThenInternalServerErrorReturned()
        {
            // Arrange 
            var leaseId = 1234578;
            var exception = new Exception("Random error encountered!");
            var expected = new ProblemDetails()
            {
                Title = $"An unknown exception occurred while attempting to retrieve saved settlement records for {nameof(leaseId)} {leaseId}.",
                Detail = "See logged errors for more details.",
                Status = StatusCodes.Status500InternalServerError
            };

            _mockVPayTransactionProvider
                .Setup(p => p.GetSettlementTransactionsAsync(It.IsAny<int>()))
                .ThrowsAsync(exception);

            // Act
            var actual = (ObjectResult)await _controller.GetSettlementTransactionsAsync(leaseId, _mockVPayTransactionProvider.Object);

            // Assert
            actual.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            actual.Value.Should().BeEquivalentTo(expected);
            _mockLogger.Verify(l => l.Log(LogLevel.Error,
                                          $"An exception occurred while retrieving saved settlement records for {nameof(leaseId)} {leaseId}!",
                                          exception,
                                          null),
                                    Times.Once);
        }
        #endregion GetSettlementTransactionsAsync Tests

        #region PostSettlementTransactionAsync Tests
        [Test]
        public async Task PostSettlementTransactionAsync_WhenSuccessfulInsertOfTransactions_Then201Returned()
        {
            // Arrange
            SetRequestMethodAndRoute("POST", "/vcard/transaction/settlement");

            var request = new SettlementTransactionRequest()
            {
                SettlementTransactions = new List<SettlementTransaction>
                {
                    new SettlementTransaction()
                    {
                        LeaseId = 123456,
                        ProviderCardId = "MyTestCard",
                        StoreId = 1234,
                        ProviderTransactionIdentifier = "MyTestTranId1333",
                        TransactionAmount = 100.00m,
                        PostedDate = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        TransactionType = "C"
                    }
                }
            };

            var expectedLoggedBody = JsonSerializer.Serialize(request);

            _mockVPayTransactionProvider
                .Setup(p => p.AddSettlementTransactionAsync(It.IsAny<SettlementTransactionRequest>()));

            // Act
            var response = (StatusCodeResult)await _controller.PostSettlementTransactionAsync(request, _mockVPayTransactionProvider.Object);
            var actualStatusCode = response.StatusCode;

            // Assert
            actualStatusCode.Should().Be(StatusCodes.Status201Created);
            _mockLogger.Verify(
                l => l.Log(
                        LogLevel.Info,
                        ExpectedRequestMethodWithRoute,
                        null,
                        new Dictionary<string, object> { { nameof(SettlementTransactionRequest), expectedLoggedBody }, }),
                Times.Once);
        }

        [Test]
        public async Task PostSettlementTransactionAsync_WhenUnexpectedErrorSavingTransactions_ThenStandard500Returned()
        {
            // Arrange
            var request = new SettlementTransactionRequest();

            var expectedLoggedBody = JsonSerializer.Serialize(request);
            var expectedResponseBody = new ProblemDetails()
            {
                Title = "An unknown exception occurred while attempting to save settlement transactions.",
                Status = 500,
                Detail = "See logged errors for more details."
            };

            _mockVPayTransactionProvider
                .Setup(p => p.AddSettlementTransactionAsync(It.IsAny<SettlementTransactionRequest>()))
                .ThrowsAsync(new Exception("Random error"));

            // Act
            var response = (ObjectResult)await _controller.PostSettlementTransactionAsync(request, _mockVPayTransactionProvider.Object);
            var actualStatusCode = response.StatusCode;
            var actualResponseBody = response.Value;

            // Assert
            actualStatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            actualResponseBody.Should().BeEquivalentTo(expectedResponseBody);
            _mockLogger.Verify(
                l => l.Log(
                        LogLevel.Error,
                        It.Is<string>(s => s.Contains("An exception occurred while processing the SettlementTransactionRequest")),
                        It.IsAny<Exception>(),
                        new Dictionary<string, object> { { nameof(SettlementTransactionRequest), expectedLoggedBody }, }),
                Times.Once);
        }

        [TestCase("Random errors! AggregateException does not include tilde.", "Random errors! AggregateException does not include tilde.")]
        [TestCase("Part of error before tilde returned! ~ AggregateException does include tilde.", "Part of error before tilde returned! ")]
        public async Task PostSettlementTransactionAsync_WhenSaveReturnsAggregateException_Then500ReturnedWithDetails(string errorMessage, string expectedDetail)
        {
            // Arrange
            var request = new SettlementTransactionRequest();

            var expectedResponseBody = new ProblemDetails()
            {
                Title = "An error occurred saving some or all settlement transactions.",
                Status = 500,
                Detail = expectedDetail
            };

            _mockVPayTransactionProvider
                .Setup(p => p.AddSettlementTransactionAsync(It.IsAny<SettlementTransactionRequest>()))
                .ThrowsAsync(new AggregateException(errorMessage));

            // Act
            var response = (ObjectResult)await _controller.PostSettlementTransactionAsync(request, _mockVPayTransactionProvider.Object);
            var actualStatusCode = response.StatusCode;
            var actualResponseBody = response.Value;

            // Assert
            actualStatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            actualResponseBody.Should().BeEquivalentTo(expectedResponseBody);
            _mockLogger.Verify(
                l => l.Log(
                        LogLevel.Error,
                        It.Is<string>(s => s.Contains("An error occurred saving some or all settlement transactions")),
                        It.IsAny<AggregateException>(),
                        null),
                Times.Once);
        }
        #endregion PostSettlementTransactionAsync Tests

        #region GetVCardsAsync Tests
        [Test]
        public async Task GetVCardsAsync_LogsCorrectly_And_Returns_200_WithCorrectBody()
        {
            // Arrange
            var expectedRequest = new GetVCardsRequest();
            var expectedLogCount = 2;
            var expectedStatusCode = StatusCodes.Status200OK;
            var expectedBody = new GetVCardsResponse
            {
                VCards = new List<VCard>
                {
                    { new VCard() }
                }
            };

            _mockVirtualCardProvider
                .Setup(p => p.GetVCardsByFilterAsync(expectedRequest))
                .ReturnsAsync(expectedBody);

            // Act
            var response = (ObjectResult)await _controller.GetVCardsAsync(expectedRequest);
            var actualStatusCode = response.StatusCode;
            var actualBody = response.Value;

            // Assert
            Assert.AreEqual(actualBody, expectedBody);
            Assert.AreEqual(actualStatusCode, expectedStatusCode);
            _mockLogger.Verify(l => l.Log(LogLevel.Info, It.IsAny<string>(), null,
                It.IsAny<IDictionary<string, object>>()), Times.Exactly(expectedLogCount));
        }

        [Test]
        public async Task GetVCardsAsync_LogsException_And_Returns_500()
        {
            // Arrange
            var expectedRequest = new GetVCardsRequest();
            var expectedInfoLogCount = 1;
            var expectedExLogCount = 1;
            var expectedStatusCode = StatusCodes.Status500InternalServerError;
            var expectedException = new Exception("Ah");

            _mockVirtualCardProvider
                .Setup(p => p.GetVCardsByFilterAsync(expectedRequest))
                .ThrowsAsync(expectedException);

            // Act
            var response = (ObjectResult)await _controller.GetVCardsAsync(expectedRequest);
            var actualStatusCode = response.StatusCode;

            // Assert
            Assert.AreEqual(actualStatusCode, expectedStatusCode);
            _mockLogger.Verify(l => l.Log(LogLevel.Error, It.IsAny<string>(), expectedException,
                It.IsAny<IDictionary<string, object>>()), Times.Exactly(expectedExLogCount));
            _mockLogger.Verify(l => l.Log(LogLevel.Info, It.IsAny<string>(), null,
                It.IsAny<IDictionary<string, object>>()), Times.Exactly(expectedInfoLogCount));
        }
        #endregion GetVCardsAsync Tests
    }
}