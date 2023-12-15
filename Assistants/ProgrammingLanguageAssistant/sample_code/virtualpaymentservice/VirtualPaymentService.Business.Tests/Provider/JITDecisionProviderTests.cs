using Moq;
using NUnit.Framework;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Provider;
using VirtualPaymentService.Data;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.Tests
{
    [TestFixture]
    public class JITDecisionProviderTests
    {
        private Mock<ILogger<IJITDecisionProvider>> _mockLogger;
        private Mock<IJITDecisionLogRepository> _mockJITLogRepository;
        private Mock<IVCardRepository> _mockVCardRepository;
        private JITDecisionProvider _provider;

        [SetUp]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger<IJITDecisionProvider>>();
            _mockJITLogRepository = new Mock<IJITDecisionLogRepository>();
            _mockVCardRepository = new Mock<IVCardRepository>();

            _provider = new JITDecisionProvider
            (
                _mockLogger.Object,
                _mockJITLogRepository.Object,
                _mockVCardRepository.Object
            );
        }

        [Test]
        public void VCard_NotFound_Returns_HttpNotFoundException()
        {
            //Arrange
            var expected = new HttpRequestException("Failed to locate a virtual card for LeaseId 123456 and ProviderCardId e143a1c4-dbd2-4c25-9bee-3bff4d7a415b.", null, HttpStatusCode.NotFound);

            var request = new JITFundingRequest()
            {
                LeaseId = 123456,
                ProviderCardId = "e143a1c4-dbd2-4c25-9bee-3bff4d7a415b",
                IsMinAmountRequired = false,
                LeaseStatus = "VCardAvailable",
                UseStateValidation = false,
                TransactionAmount = 1.00m,
                TransactionDate = DateTime.UtcNow,
                TransactionType = "AUTHORIZATION"
            };

            _mockVCardRepository
                .Setup(r => r.GetVCardAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync((VCard)null);

            //Act
            AsyncTestDelegate testDelegate = async () =>
            {
                await _provider.ProcessJITDecisionAsync(request);
            };

            //Assert
            var actual = Assert.ThrowsAsync<HttpRequestException>(async () => await testDelegate());
            Assert.AreEqual(expected.StatusCode, actual.StatusCode);
            Assert.AreEqual(expected.Message, actual.Message);
        }

        [Test]
        public void ExceptionThrown_Returns_SameException()
        {
            //Arrange
            var expected = new Exception("Random Error Calling Procedure");

            var request = new JITFundingRequest()
            {
                LeaseId = 123456,
                ProviderCardId = "e143a1c4-dbd2-4c25-9bee-3bff4d7a415b",
                IsMinAmountRequired = false,
                LeaseStatus = "VCardAvailable",
                UseStateValidation = false,
                TransactionAmount = 1.00m,
                TransactionDate = DateTime.UtcNow,
                TransactionType = "AUTHORIZATION"
            };

            _mockVCardRepository
                .Setup(r => r.GetVCardAsync(It.IsAny<long>(), It.IsAny<string>()))
                .Throws(expected);

            //Act
            AsyncTestDelegate testDelegate = async () =>
            {
                await _provider.ProcessJITDecisionAsync(request);
            };

            //Assert
            var actual = Assert.ThrowsAsync<Exception>(async () => await testDelegate());
            Assert.AreEqual(expected.Message, actual.Message);
        }

        [Test]
        public async Task Any_VCard_Found_Returns_SuccessTrue()
        {
            //Arrange
            var vCard = new VCard()
            {
                LeaseId = 123456,
                ActiveToDate = DateTime.UtcNow.AddDays(1),
                VCardStatusId = CardStatus.Open,
                AvailableBalance = 2.00m
            };
            var request = new JITFundingRequest()
            {
                LeaseId = vCard.LeaseId,
                ProviderCardId = "e143a1c4-dbd2-4c25-9bee-3bff4d7a415b",
                IsMinAmountRequired = false,
                LeaseStatus = "VCardAvailable",
                UseStateValidation = false,
                TransactionAmount = 1.00m,
                TransactionDate = DateTime.UtcNow,
                TransactionType = "AUTHORIZATION"
            };

            _mockVCardRepository
                .Setup(r => r.GetVCardAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(vCard);

            //Act
            var response = await _provider.ProcessJITDecisionAsync(request);

            //Assert
            Assert.IsTrue(response.Approved);
        }

        [Test]
        public async Task When_ProcessJITDecisionAsync_Called_With_MisMatched_State_ThenDeclineLogged()
        {
            //Arrange
            var vCard = new VCard()
            {
                LeaseId = 123456,
                ActiveToDate = DateTime.UtcNow.AddDays(1),
                VCardStatusId = CardStatus.Open,
                AvailableBalance = 2.00m
            };
            var request = new JITFundingRequest()
            {
                LeaseId = vCard.LeaseId,
                ProviderCardId = "e143a1c4-dbd2-4c25-9bee-3bff4d7a415b",
                IsMinAmountRequired = false,
                LeaseStatus = "VCardAvailable",
                UseStateValidation = true,
                TransactionAmount = 1.00m,
                TransactionDate = DateTime.UtcNow,
                TransactionType = "AUTHORIZATION",
                TransactionState = "UT",
                StoreAddressState = "DE"
            };

            _mockVCardRepository
                .Setup(r => r.GetVCardAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(vCard);

            //Act
            var response = await _provider.ProcessJITDecisionAsync(request);

            //Assert
            Assert.IsFalse(response.Approved);
            _mockLogger.Verify(logger =>
                    logger.Log(
                        LogLevel.Info,
                        It.Is<string>(s =>
                            s.Contains(
                                $"declined, Decline Reason: State does not match the lease State")),
                        It.IsAny<Exception>(),
                        It.IsAny<Dictionary<string, object>>()
                    ),
                Times.Once()
            );
        }

        [Test]
        public async Task When_ProcessJITDecisionAsync_Called_With_AuthApproval_ThenApprovedLogWritten()
        {
            //Arrange
            var vCard = new VCard()
            {
                LeaseId = 123456,
                ActiveToDate = DateTime.UtcNow.AddDays(1),
                VCardStatusId = CardStatus.Open,
                AvailableBalance = 2.00m
            };
            var request = new JITFundingRequest()
            {
                LeaseId = vCard.LeaseId,
                ProviderCardId = "e143a1c4-dbd2-4c25-9bee-3bff4d7a415b",
                IsMinAmountRequired = false,
                LeaseStatus = "VCardAvailable",
                UseStateValidation = true,
                TransactionAmount = 1.00m,
                TransactionDate = DateTime.UtcNow,
                TransactionType = "AUTHORIZATION",
                TransactionState = "UT",
                StoreAddressState = "UT"
            };

            _mockVCardRepository
                .Setup(r => r.GetVCardAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(vCard);

            //Act
            var response = await _provider.ProcessJITDecisionAsync(request);

            //Assert
            Assert.IsTrue(response.Approved);
            _mockLogger.Verify(logger =>
                    logger.Log(
                        LogLevel.Info,
                        It.Is<string>(s => s.Contains($"was approved")),
                        It.IsAny<Exception>(),
                        It.IsAny<Dictionary<string, object>>()
                    ),
                Times.Once()
            );
        }
    }
}
