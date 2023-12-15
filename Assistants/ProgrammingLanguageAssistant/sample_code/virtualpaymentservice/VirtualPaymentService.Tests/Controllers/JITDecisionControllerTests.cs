using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Controllers;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Tests
{
    [TestFixture]
    public class JITDecisionControllerTests
    {
        private Mock<IJITDecisionProvider> _mockJITProvider;
        private Mock<ILogger<JITDecisionController>> _mockLogger;
        private JITDecisionController _controller;

        [SetUp]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger<JITDecisionController>>();
            _mockJITProvider = new Mock<IJITDecisionProvider>();
            _controller = new JITDecisionController(_mockLogger.Object, _mockJITProvider.Object);
        }

        [Test]
        public async Task JITDecision_Post_Exception_ReturnsStatus500()
        {
            // Arrange
            _mockJITProvider
                .Setup(p => p.ProcessJITDecisionAsync(It.IsAny<JITFundingRequest>()))
                .Throws(new Exception());

            // Act
            var result = await _controller.Post(It.IsAny<JITFundingRequest>()) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Test]
        public async Task JITDecision_WhenNoCardFoundForLease_ThenReturnStatus404()
        {
            // Arrange
            var expected = new HttpRequestException("Failed to locate a virtual card for LeaseId 123456 and ProviderCardId e143a1c4-dbd2-4c25-9bee-3bff4d7a415b.", null, HttpStatusCode.NotFound);

            _mockJITProvider
                .Setup(p => p.ProcessJITDecisionAsync(It.IsAny<JITFundingRequest>()))
                .Throws(expected);

            // Act
            var actual = await _controller.Post(It.IsAny<JITFundingRequest>()) as ObjectResult;

            // Assert
            Assert.AreEqual((int)expected.StatusCode, actual.StatusCode);
        }

        [Test]
        public async Task JITDecision_WhenHttpExceptionDoesNotContainStatusCode_ThenReturnStatus500()
        {
            // Arrange
            _mockJITProvider
                .Setup(p => p.ProcessJITDecisionAsync(It.IsAny<JITFundingRequest>()))
                .Throws(new HttpRequestException("Random HTTP error without status code", null, null));

            // Act
            var actual = await _controller.Post(It.IsAny<JITFundingRequest>()) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actual.StatusCode);
        }

        [Test]
        public async Task JITDecision_Post_ReturnsStatus201()
        {
            // Arrange
            _mockJITProvider
                .Setup(p => p.ProcessJITDecisionAsync(It.IsAny<JITFundingRequest>()))
                .ReturnsAsync(It.IsAny<JITFundingResponse>());

            // Act
            var result = await _controller.Post(It.IsAny<JITFundingRequest>()) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        }
    }
}