using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Controllers;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Tests
{
    [TestFixture]
    public class LeaseStoreDataControllerTests
    {
        private Mock<ILeaseStoreDataProvider> _mockDataProvider;
        private Mock<ILogger<LeaseStoreDataController>> _mockLogger;
        private LeaseStoreDataController _controller;

        [SetUp]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger<LeaseStoreDataController>>();
            _mockDataProvider = new Mock<ILeaseStoreDataProvider>();
            _controller = new LeaseStoreDataController(_mockLogger.Object, _mockDataProvider.Object);
        }

        [Test]
        public async Task LeaseStore_Get_Exception_ReturnsStatus500()
        {
            // Arrange
            _mockDataProvider
                .Setup(p => p.GetLeaseStoreData(It.IsAny<LeaseStoreDataRequest>()))
                .Throws<Exception>();

            // Act
            var result = await _controller.Get(It.IsAny<LeaseStoreDataRequest>()) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Test]
        public async Task LeaseStore_Get_ReturnsStatus200()
        {
            // Arrange
            _mockDataProvider
                .Setup(p => p.GetLeaseStoreData(It.IsAny<LeaseStoreDataRequest>()))
                .Returns(It.IsAny<LeaseStoreDataResponse>());

            // Act
            var result = await _controller.Get(It.IsAny<LeaseStoreDataRequest>()) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [Test]
        public async Task LeaseStore_GetMarqetaState_Exception_ReturnsStatus500()
        {
            // Arrange
            _mockDataProvider
                .Setup(p => p.GetMarqetaStateAbbreviation(It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var result = await _controller.GetMarqetaStateId(It.IsAny<int>()) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Test]
        public async Task GetMarqetaState_ReturnsStatus200()
        {
            // Arrange
            _mockDataProvider
                .Setup(p => p.GetMarqetaStateAbbreviation(It.IsAny<int>()))
                .Returns(It.IsAny<MarqetaStateResponse>());

            // Act
            var result = await _controller.GetMarqetaStateId(It.IsAny<int>()) as ObjectResult;

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        #region GetStoreCustomFields Tests
        [Test]
        public async Task GetStoreCustomFields_Returns200IfVCardIsNotNull()
        {
            // Assemble
            _mockDataProvider
                .Setup(p => p.GetStoreCustomFields(It.IsAny<int>()))
                .ReturnsAsync(new List<StoreCustomField>());

            // Act
            var response = (ObjectResult)await _controller.GetStoreCustomFields(123456);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
        }

        [Test]
        public async Task GetStoreCustomFields_Returns500WhenException()
        {
            // Assemble
            _mockDataProvider
                .Setup(p => p.GetStoreCustomFields(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            // Act
            var response = (ObjectResult)await _controller.GetStoreCustomFields(123456);

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
        }
        #endregion GetStoreCustomFields Tests
    }
}