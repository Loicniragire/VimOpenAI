using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Common.AutoMapper;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Provider;
using VirtualPaymentService.Data.Facade;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.DTO.Database.Result;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Types;

namespace VirtualPaymentService.Business.Tests.Provider
{
    [TestFixture]
    public class VPayTransactionProviderTests
    {
        #region Field
        private Mock<ILogger<IVPayTransactionProvider>> mockLogger;
        private Mock<IVPayTransactionRepository> mockVPayTransactionRepository;
        private IMapper mapper;
        private IVPayTransactionProvider trxProvider;
        #endregion Field

        #region SetUp
        [SetUp]
        public void SetUp()
        {
            mockLogger = new Mock<ILogger<IVPayTransactionProvider>>();
            mockVPayTransactionRepository = new Mock<IVPayTransactionRepository>();
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<VirtualCardProfile>();
            }).CreateMapper();

            trxProvider = new VPayTransactionProvider(
                mockLogger.Object,
                mapper,
                mockVPayTransactionRepository.Object);
        }
        #endregion SetUp

        #region GetVirtualCardAuthorizationsAsync Tests
        [Test]
        public async Task GetVirtualCardAuthorizationsAsync_WhenNoAuthsLocated_ThenReturnEmptyResponse()
        {
            // Arrange
            var expected = new GetVirtualCardAuthorizationsResponse();

            mockVPayTransactionRepository
                .Setup(repo => repo.GetVCardPurchaseAuthAsync(It.IsAny<int>()))
                // No auths located
                .ReturnsAsync(new List<VCardPurchaseAuthResult>());

            // Act
            var actual = await trxProvider.GetVirtualCardAuthorizationsAsync(It.IsAny<int>());

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetVirtualCardAuthorizationsAsync_WhenAuthsLocated_ThenResponseProperlyMapped()
        {
            // Arrange
            var denialInternalReasonFromDb = new VCardPurchaseAuthResult()
            {
                Amount = 110.00m,
                ApprovalCode = string.Empty,
                AuthorizationDateTime = DateTime.UtcNow,
                DeclineReasonCode = "1016",
                ProgressiveDeclineReasonMessage = "Internal reason message from JIT Funding process.",
                DeclineReasonMessage = "Reason from card provider.",
                Response = "Decline",
                MerchantName = "Lowes.com",
                TypeCode = "0100",
                TypeDesc = "Description",
                CategoryCode = "5200"
            };

            var denialExternalReasonFromDb = new VCardPurchaseAuthResult()
            {
                Amount = 120.00m,
                ApprovalCode = string.Empty,
                AuthorizationDateTime = DateTime.UtcNow,
                DeclineReasonCode = "1017",
                ProgressiveDeclineReasonMessage = string.Empty,
                DeclineReasonMessage = "Reason from card provider.",
                Response = "Decline",
                MerchantName = "Lowes.com",
                TypeCode = "0100",
                TypeDesc = "Description",
                CategoryCode = "5200"
            };

            var approvalFromDb = new VCardPurchaseAuthResult()
            {
                Amount = 100.00m,
                ApprovalCode = "133798",
                AuthorizationDateTime = DateTime.UtcNow,
                DeclineReasonCode = string.Empty,
                DeclineReasonMessage = string.Empty,
                ProgressiveDeclineReasonMessage = string.Empty,
                Response = "Approval",
                MerchantName = "BestBuy.com",
                TypeCode = "0100",
                TypeDesc = "Description",
                CategoryCode = "5200"
            };

            var vCardPurchaseAuthResultFromDb = new List<VCardPurchaseAuthResult>
            {
                denialInternalReasonFromDb,
                denialExternalReasonFromDb,
                approvalFromDb
            };

            var expected = new GetVirtualCardAuthorizationsResponse();

            expected.Authorizations.Add(new VirtualCardAuthorization
            {
                Amount = denialInternalReasonFromDb.Amount,
                ApprovalCode = denialInternalReasonFromDb.ApprovalCode,
                AuthorizationDateTime = denialInternalReasonFromDb.AuthorizationDateTime,
                DeclineReasonCode = denialInternalReasonFromDb.DeclineReasonCode,
                // Since we have a prog reason it should be the text returned.
                DeclineReasonMessage = denialInternalReasonFromDb.ProgressiveDeclineReasonMessage,
                Response = denialInternalReasonFromDb.Response,
                MerchantName = denialInternalReasonFromDb.MerchantName,
                TypeCode = denialInternalReasonFromDb.TypeCode,
                TypeDesc = denialInternalReasonFromDb.TypeDesc,
                MerchantCategoryCode = denialInternalReasonFromDb.CategoryCode
            });

            expected.Authorizations.Add(new VirtualCardAuthorization
            {
                Amount = denialExternalReasonFromDb.Amount,
                ApprovalCode = denialExternalReasonFromDb.ApprovalCode,
                AuthorizationDateTime = denialExternalReasonFromDb.AuthorizationDateTime,
                DeclineReasonCode = denialExternalReasonFromDb.DeclineReasonCode,
                // Since we dont have a prog reason it should be the provider text.
                DeclineReasonMessage = denialExternalReasonFromDb.DeclineReasonMessage,
                Response = denialExternalReasonFromDb.Response,
                MerchantName = denialExternalReasonFromDb.MerchantName,
                TypeCode = denialExternalReasonFromDb.TypeCode,
                TypeDesc = denialExternalReasonFromDb.TypeDesc,
                MerchantCategoryCode = denialExternalReasonFromDb.CategoryCode
            });

            expected.Authorizations.Add(new VirtualCardAuthorization
            {
                Amount = approvalFromDb.Amount,
                ApprovalCode = approvalFromDb.ApprovalCode,
                AuthorizationDateTime = approvalFromDb.AuthorizationDateTime,
                DeclineReasonCode = approvalFromDb.DeclineReasonCode,
                DeclineReasonMessage = approvalFromDb.DeclineReasonMessage,
                Response = approvalFromDb.Response,
                MerchantName = approvalFromDb.MerchantName,
                TypeCode = approvalFromDb.TypeCode,
                TypeDesc = approvalFromDb.TypeDesc,
                MerchantCategoryCode = approvalFromDb.CategoryCode
            });

            mockVPayTransactionRepository
                .Setup(repo => repo.GetVCardPurchaseAuthAsync(It.IsAny<int>()))
                .ReturnsAsync(vCardPurchaseAuthResultFromDb);

            // Act
            var actual = await trxProvider.GetVirtualCardAuthorizationsAsync(It.IsAny<int>());

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
        #endregion #region GetVirtualCardAuthorizationsAsync Tests

        #region AddVCardAuthorizationAsync Tests
        [TestCase(Constants.Authorizations.DbDuplicateAuthMessage, TestName = "AddVCardAuthorizationAsync_When_DuplicateAuth_SoftFails")]
        [TestCase(Constants.Authorizations.DbMissingVCardMessage, TestName = "AddVCardAuthorizationAsync_When_VCardNotFound_SoftFails")]
        public async Task AddVCardAuthorizationAsync_SoftFails_OnSpecificIssues(string exMessage)
        {
            // Arrange
            var ex = new MockDbException(exMessage);
            var request = new VCardPurchaseAuthRequest
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta
            };
            mockVPayTransactionRepository
                .Setup(repo => repo.InsertVCardPurchaseAuthAsync(It.IsAny<InsertVCardPurchaseAuthParams>()))
                .ThrowsAsync(ex);

            // Act
            var result = await trxProvider.AddVCardAuthorizationAsync(request);

            // Assert
            Assert.IsTrue(result.SoftFail);
        }

        [Test]
        public void AddVCardAuthorizationAsync_WhenNotASoftFailException_Rethrows()
        {
            // Arrange
            var request = new VCardPurchaseAuthRequest
            {
                VCardProviderId = VirtualCardProviderNetwork.Marqeta
            };
            mockVPayTransactionRepository
                .Setup(repo => repo.InsertVCardPurchaseAuthAsync(It.IsAny<InsertVCardPurchaseAuthParams>()))
                .ThrowsAsync(new MockDbException("AH!"));

            // Act & Assert
            Assert.ThrowsAsync<MockDbException>(() => trxProvider.AddVCardAuthorizationAsync(request));
        }

        [TestCase(Constants.Authorizations.Approval, Constants.Authorizations.AuthorizationType, CardStatus.Open, CardStatus.Authorized, 200, 300, 0, 100, 200, TestName = "Approved_Authorization_ForOpenVCard_ResultsIn_AuthorizedVCard_And_AdjustedBalances")]
        [TestCase(Constants.Authorizations.Approval, Constants.Authorizations.AuthorizationType, CardStatus.Authorized, CardStatus.Authorized, 200, 300, 0, 100, 200, TestName = "Approved_Authorization_ForAuthorizedVCard_ResultsIn_AuthorizedVCard_And_AdjustedBalances")]
        [TestCase(Constants.Authorizations.Approval, Constants.Authorizations.AuthorizationType, CardStatus.Cancelled, CardStatus.Cancelled, 200, 300, 0, 100, 200, TestName = "Approved_Authorization_ForCancelledVCard_ResultsIn_CancelledVCard_And_AdjustedBalances")]
        [TestCase(Constants.Authorizations.Approval, Constants.Authorizations.ReversalType, CardStatus.Authorized, CardStatus.Open, 200, 300, 200, 500, 0, TestName = "Approved_FullReversal_ForAuthorizedVCard_ResultsIn_OpenVCard_And_AdjustedBalances")]
        [TestCase(Constants.Authorizations.Approval, Constants.Authorizations.ReversalType, CardStatus.Authorized, CardStatus.Authorized, 199, 300, 200, 499, 1, TestName = "Approved_PartialReversal_ForAuthorizedVCard_ResultsIn_AuthorizedVCard_And_AdjustedBalances")]
        [TestCase(Constants.Authorizations.Decline, Constants.Authorizations.AuthorizationType, CardStatus.Open, CardStatus.Open, 200, 300, 0, 300, 0, TestName = "Declined_Authorization_ForOpenVCard_ResultsIn_OpenVCard_And_OriginalBalances")]
        [TestCase(Constants.Authorizations.Decline, Constants.Authorizations.AuthorizationType, CardStatus.Authorized, CardStatus.Authorized, 200, 300, 100, 300, 100, TestName = "Declined_Authorization_ForAuthorizedVCard_ResultsIn_AuthorizedVCard_And_OriginalBalances")]
        [TestCase(Constants.Authorizations.Decline, Constants.Authorizations.ReversalType, CardStatus.Open, CardStatus.Open, 200, 300, 0, 300, 0, TestName = "Declined_Reversal_ForOpenVCard_ResultsIn_OpenVCard_And_OriginalBalances")]
        [TestCase(Constants.Authorizations.Decline, Constants.Authorizations.ReversalType, CardStatus.Authorized, CardStatus.Authorized, 200, 300, 100, 300, 100, TestName = "Declined_Reversal_ForAuthorizedVCard_ResultsIn_AuthorizedVCard_And_OriginalBalances")]
        public async Task AddVCardAuthorizationAsync_HappyPath_HandlesRequest_Properly(
            string approvalResponse,
            string authType,
            CardStatus originalVCardStatus,
            CardStatus expectedVCardStatus,
            decimal authAmount,
            decimal originalAvailableBalance,
            decimal originalCardBalance,
            decimal expectedAvailableBalance,
            decimal expectedCardBalance)
        {
            // Arrange
            var request = new VCardPurchaseAuthRequest
            {
                Amount = authAmount,
                ApprovalCode = "1244",
                AuthorizationDateTime = DateTime.Now,
                AuthorizationId = Guid.NewGuid().ToString(),
                DeclineReasonCode = "3412",
                DeclineReasonMessage = "Whoops",
                MerchantCategoryCode = "2341",
                MerchantCity = "Draper",
                MerchantCountry = "US",
                MerchantId = Guid.NewGuid().ToString(),
                MerchantName = "Rick Rolled",
                MerchantPostalCode = "13241",
                MerchantStateProvince = "UT",
                ProgressiveDeclineReasonMessage = "You know why",
                ReferenceId = Guid.NewGuid().ToString(),
                Response = approvalResponse,
                SourceCurrencyCode = "USD",
                TypeCode = authType,
                TypeDesc = "My little description",
                VCardProviderId = VirtualCardProviderNetwork.Marqeta
            };

            var expectedInsertAuthParams = mapper.Map<InsertVCardPurchaseAuthParams>(request);
            var expectedVCardId = 92;
            var insertAuthResult = new InsertVCardPurchaseAuthResult
            {
                VCardPurchaseAuthId = 4327,
                LeaseId = 12345,
                VCardHistoryId = 123,
                VCardId = expectedVCardId
            };
            var vCard = new VCard
            {
                AvailableBalance = originalAvailableBalance,
                CardBalance = originalCardBalance,
                VCardStatusId = originalVCardStatus,
                VCardId = 1234,
                ReferenceId = request.ReferenceId,
                LeaseId = 7419679
            };

            var expectedVCardUpdateParams = mapper.Map<VCardUpdateParams>(vCard);
            expectedVCardUpdateParams.VCardStatusId = expectedVCardStatus;
            expectedVCardUpdateParams.AvailableBalance = expectedAvailableBalance;
            expectedVCardUpdateParams.CardBalance = expectedCardBalance;

            mockVPayTransactionRepository
                .Setup(repo => repo.InsertVCardPurchaseAuthAsync(It.IsAny<InsertVCardPurchaseAuthParams>()))
                .Callback<InsertVCardPurchaseAuthParams>(authParams =>
                {
                    authParams.Should().BeEquivalentTo(expectedInsertAuthParams);
                })
                .ReturnsAsync(insertAuthResult);

            mockVPayTransactionRepository
                .Setup(repo => repo.GetVCardByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(vCard);

            // Only called when the Response of the auth is Approval
            mockVPayTransactionRepository
                .Setup(repo => repo.UpdateVCardAsync(It.IsAny<VCardUpdateParams>()))
                .Callback<VCardUpdateParams>(vCardParams =>
                {
                    Assert.AreEqual(Constants.Authorizations.Approval, request.Response);
                    vCardParams.Should().BeEquivalentTo(expectedVCardUpdateParams);
                });

            // Act
            var result = await trxProvider.AddVCardAuthorizationAsync(request);
            var cardData = result.Response;

            // Assert
            mockVPayTransactionRepository
                .Verify(repo => repo.GetVCardByIdAsync(expectedVCardId), Times.Once());
            Assert.IsFalse(result.SoftFail);
            Assert.AreEqual(expectedCardBalance, cardData.CardBalance);
            Assert.AreEqual(expectedAvailableBalance, cardData.AvailableBalance);
            Assert.AreEqual(expectedVCardStatus, cardData.VCardStatusId);
            Assert.AreEqual(request.ReferenceId, cardData.ReferenceId);
        }
        #endregion AddVCardAuthorizationAsync Tests

        #region GetSettlementTransactionsAsync Tests
        [Test]
        public async Task GetSettlementTransactionsAsync_WhenSettlementTransactionssNotLocated_ThenReturnEmptyResponse()
        {
            // Arrange
            var expected = new GetSettlementTransactionsResponse();

            mockVPayTransactionRepository
                .Setup(repo => repo.GetSettlementTransactionAsync(It.IsAny<int>()))
                // No settlements located
                .ReturnsAsync(new List<StoreVPayReconciliation>());

            // Act
            var actual = await trxProvider.GetSettlementTransactionsAsync(It.IsAny<int>());

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetSettlementTransactionsAsync_WhenSettlementTransactionsLocated_ThenReturnResponse()
        {
            // Arrange
            var leaseId = 1234546;
            var settlementTransactionsFromDb = new List<StoreVPayReconciliation>
            {
                new StoreVPayReconciliation { LeaseId = leaseId },
                new StoreVPayReconciliation { LeaseId = leaseId }
            };
            var expected = new GetSettlementTransactionsResponse() { Settlements = settlementTransactionsFromDb };

            mockVPayTransactionRepository
                .Setup(repo => repo.GetSettlementTransactionAsync(leaseId))
                .ReturnsAsync(settlementTransactionsFromDb);

            // Act
            var actual = await trxProvider.GetSettlementTransactionsAsync(leaseId);

            // Assert
            actual.Should().BeEquivalentTo(expected);

        }
        #endregion GetSettlementTransactionsAsync Tests

        #region AddSettlementTransactionAsync Tests
        [Test]
        public async Task AddSettlementTransactionAsync_WhenMultipleTransactionsInRequest_ThenEachTranSavedIndividually()
        {
            // Arrange
            var expectedLeaseIds = new List<int> { 123456, 789012 };
            var actualLeaseIds = new List<int>();

            var request = new SettlementTransactionRequest()
            {
                SettlementTransactions = new List<SettlementTransaction>
                {
                    new SettlementTransaction()
                    {
                        LeaseId = expectedLeaseIds[0]
                    },
                    new SettlementTransaction()
                    {
                        LeaseId = expectedLeaseIds[1]
                    }
                }
            };

            mockVPayTransactionRepository
                .Setup(repo => repo.InsertSettlementTransactionAsync(It.IsAny<SettlementTransaction>()))
                .Callback<SettlementTransaction>(transaction =>
                {
                    actualLeaseIds.Add((int)transaction.LeaseId);
                });

            // Act
            await trxProvider.AddSettlementTransactionAsync(request);

            // Assert
            actualLeaseIds.Should().BeEquivalentTo(expectedLeaseIds);
        }

        [Test]
        public async Task AddSettlementTransactionAsync_WhenPreviouslySavedTransactionsInRequest_ThenSuccessfulReturnWithLoggedDetils()
        {
            // Arrange
            var request = new SettlementTransactionRequest()
            {
                SettlementTransactions = new List<SettlementTransaction>
                {
                    new SettlementTransaction()
                    {
                        LeaseId = 123456,
                        ProviderCardId = "MyTestCard1",
                        StoreId = 1234,
                        ProviderTransactionIdentifier = "MyTestTranId1333",
                        TransactionAmount = 100.00m,
                        PostedDate = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        TransactionType = "C"
                    },
                    new SettlementTransaction()
                    {
                        LeaseId = 789012,
                        ProviderCardId = "MyTestCard2",
                        StoreId = 5678,
                        ProviderTransactionIdentifier = "MyTestTranId1444",
                        TransactionAmount = 200.00m,
                        PostedDate = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        TransactionType = "D"
                    }
                }
            };

            var expectedLoggedBodyFirstLease = JsonSerializer.Serialize(request.SettlementTransactions[0]);
            var expectedLoggedBodySecondLease = JsonSerializer.Serialize(request.SettlementTransactions[1]);

            mockVPayTransactionRepository
                .Setup(repo => repo.InsertSettlementTransactionAsync(It.IsAny<SettlementTransaction>()))
                // Returning the error we will get from SQL if the transaction is already in the table.
                .ThrowsAsync(new MockDbException("Cannot insert duplicate key row in object \'Store.VPayReconciliation\' with unique index \'UX_VPayReconciliation_ProviderTransactionIdentifier\'. The duplicate key value is (MyTestTranId1333)."));

            // Act
            await trxProvider.AddSettlementTransactionAsync(request);

            // Assert
            mockLogger.Verify(
                l => l.Log(
                        LogLevel.Warn,
                        It.Is<string>(s => s.Contains("Duplicate insert request for")),
                        It.IsAny<DbException>(),
                        new Dictionary<string, object> { { nameof(SettlementTransaction), expectedLoggedBodyFirstLease }, }),
                Times.Once);

            mockLogger.Verify(
                l => l.Log(
                        LogLevel.Warn,
                        It.Is<string>(s => s.Contains("Duplicate insert request for")),
                        It.IsAny<DbException>(),
                        new Dictionary<string, object> { { nameof(SettlementTransaction), expectedLoggedBodySecondLease }, }),
                Times.Once);
        }

        [Test]
        public void AddSettlementTransactionAsync_WhenSaveOfTransactionFail_ThenAggregateExceptionReturned()
        {
            // Arrange
            var request = new SettlementTransactionRequest()
            {
                SettlementTransactions = new List<SettlementTransaction>
                {
                    new SettlementTransaction()
                    {
                        LeaseId = 123456,
                        ProviderCardId = "MyTestCard1",
                        StoreId = 1234,
                        ProviderTransactionIdentifier = "MyTestTranId1333",
                        TransactionAmount = 100.00m,
                        PostedDate = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        TransactionType = "C"
                    },
                    new SettlementTransaction()
                    {
                        LeaseId = 789012,
                        ProviderCardId = "MyTestCard2",
                        StoreId = 5678,
                        ProviderTransactionIdentifier = "MyTestTranId1444",
                        TransactionAmount = 200.00m,
                        PostedDate = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        TransactionType = "D"
                    }
                }
            };

            var expectedLoggedBodyFirstLease = JsonSerializer.Serialize(request.SettlementTransactions[0]);
            var expectedLoggedBodySecondLease = JsonSerializer.Serialize(request.SettlementTransactions[1]);

            mockVPayTransactionRepository
                .Setup(repo => repo.InsertSettlementTransactionAsync(It.IsAny<SettlementTransaction>()))
                // Throwing error that will be added to the AggregateException
                .ThrowsAsync(new Exception("Random error saving transaction."));

            // Act
            AggregateException actual = Assert.ThrowsAsync<AggregateException>(async () => await trxProvider.AddSettlementTransactionAsync(request));

            // Assert
            actual.Message.Should().StartWith("2 of the 2 SettlementTransactions failed to be created. The list of ProviderTransactionIdentifier values that failed are: (MyTestTranId1333,MyTestTranId1444) ~");

            mockLogger.Verify(
                l => l.Log(
                        LogLevel.Error,
                        It.Is<string>(s => s.Contains("Failed to insert settlement transaction")),
                        It.IsAny<Exception>(),
                        new Dictionary<string, object> { { nameof(SettlementTransaction), expectedLoggedBodyFirstLease }, }),
                Times.Once);

            mockLogger.Verify(
                l => l.Log(
                        LogLevel.Error,
                        It.Is<string>(s => s.Contains("Failed to insert settlement transaction")),
                        It.IsAny<Exception>(),
                        new Dictionary<string, object> { { nameof(SettlementTransaction), expectedLoggedBodySecondLease }, }),
                Times.Once);
        }
        #endregion AddSettlementTransactionAsync Tests

        #region MockException
        class MockDbException : DbException
        {
            public MockDbException(string message) : base(message) { }
        }
        #endregion MocKException
    }
}
