using Moq;
using NUnit.Framework;
using System;
using VirtualPaymentService.Business.JIT.Rules;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.Tests
{
    [TestFixture]
    class JITFundingRuleTests
    {
        #region TransactionTypeRule Tests

        [TestCase("abcdef", Description = "TransactionType any value other than 'AUTHORIZATION'")]
        [TestCase("")]
        [TestCase(null)]
        public void TransactionTypeRule_Declines_InvalidTransactionType(string transactionType)
        {
            //Arrange
            var rule = new TransactionTypeRule();
            var mockRequest = new JITFundingRequest { TransactionType = transactionType };

            //Act
            var decision = rule.Execute(mockRequest, It.IsAny<VCard>());

            //Assert
            Assert.IsFalse(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.InvalidTransactionType, decision.DeclineReason);
        }

        [Test]
        public void TransactionTypeRule_Approves_ValidTransactionType()
        {
            //Arrange
            var rule = new TransactionTypeRule();
            var mockRequest = new JITFundingRequest { TransactionType = "AUTHORIZATION" };

            //Act
            var decision = rule.Execute(mockRequest, It.IsAny<VCard>());

            //Assert
            Assert.IsTrue(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.None, decision.DeclineReason);
        }

        #endregion TransactionTypeRule Tests

        #region ActiveToDateRule Tests

        [Test]
        public void ActiveToDateRule_Declines_ExpiredCard()
        {
            //Arrange
            var rule = new ActiveToDateRule();
            var mockRequest = new JITFundingRequest { TransactionDate = DateTime.UtcNow };
            var mockVCard = new VCard { ActiveToDate = DateTime.MinValue };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsFalse(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.CardExpired, decision.DeclineReason);
        }

        [Test]
        public void ActiveToDateRule_Approves_UnexpiredCard()
        {
            //Arrange
            var rule = new ActiveToDateRule();
            var mockRequest = new JITFundingRequest { TransactionDate = DateTime.UtcNow };
            var mockVCard = new VCard { ActiveToDate = DateTime.MaxValue };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsTrue(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.None, decision.DeclineReason);
        }

        #endregion ActiveToDateRule Tests

        #region CardStatusRule Tests

        [Test]
        public void CardStatusRule_Declines_InvalidCardStatus()
        {
            //Arrange
            var rule = new CardStatusRule();
            var mockRequest = It.IsAny<JITFundingRequest>();
            var mockVCard = new VCard { VCardStatusId = CardStatus.None };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsFalse(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.InvalidCardStatus, decision.DeclineReason);
        }

        [TestCase(CardStatus.Open)]
        [TestCase(CardStatus.Authorized)]
        public void CardStatusRule_Approves_ValidCardStatus(CardStatus cardStatus)
        {
            //Arrange
            var rule = new CardStatusRule();
            var mockRequest = It.IsAny<JITFundingRequest>();
            var mockVCard = new VCard { VCardStatusId = cardStatus };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsTrue(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.None, decision.DeclineReason);
        }

        #endregion CardStatusRule Tests

        #region AvailableBalanceRule Tests

        [Test]
        public void AvailableBalanceRule_Declines_AmountMismatch()
        {
            //Arrange
            var rule = new AvailableBalanceRule();
            var mockRequest = new JITFundingRequest
            {
                TransactionAmount = 1.00m,
                IsMinAmountRequired = true
            };
            var mockVCard = new VCard
            {
                VCardStatusId = CardStatus.Open,
                OriginalCardBaseAmount = 2.00m
            };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsFalse(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.AmountMismatch, decision.DeclineReason);
        }

        [TestCase(CardStatus.Open)]
        [TestCase(CardStatus.Authorized)]
        public void AvailableBalanceRule_Approves_AvailableBalance(CardStatus cardStatus)
        {
            //Arrange
            var rule = new AvailableBalanceRule();
            var mockRequest = new JITFundingRequest
            {
                TransactionAmount = 1.00m,
                IsMinAmountRequired = false
            };
            var mockVCard = new VCard
            {
                VCardStatusId = cardStatus,
                AvailableBalance = 2.00m
            };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsTrue(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.None, decision.DeclineReason);
        }

        [TestCase(CardStatus.Open)]
        [TestCase(CardStatus.Authorized)]
        public void AvailableBalanceRule_Declines_AmountTooHigh(CardStatus cardStatus)
        {
            //Arrange
            var rule = new AvailableBalanceRule();
            var mockRequest = new JITFundingRequest
            {
                TransactionAmount = 2.00m,
                IsMinAmountRequired = false
            };
            var mockVCard = new VCard
            {
                VCardStatusId = cardStatus,
                AvailableBalance = 1.00m
            };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsFalse(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.AmountTooHigh, decision.DeclineReason);
        }

        [Test]
        public void AvailableBalanceRule_Declines_PreviouslyAuthorizedStatus()
        {
            //Arrange
            var rule = new AvailableBalanceRule();
            var mockRequest = new JITFundingRequest
            {
                IsMinAmountRequired = true
            };
            var mockVCard = new VCard { VCardStatusId = CardStatus.Authorized };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsFalse(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.PreviouslyAuthorized, decision.DeclineReason);
        }

        [TestCase(900.00, 1000.00, 100.00, 1000.00, true, JITFundingDeclineReason.None, Description = "Approve when requested tran amount within threshold.")]
        [TestCase(1000.01, 1000.00, 100.00, 1000.00, false, JITFundingDeclineReason.AmountMismatch, Description = "Decline when requested tran amount above threshold.")]
        [TestCase(899.99, 1000.00, 100.00, 1000.00, false, JITFundingDeclineReason.AmountMismatch, Description = "Decline when requested tran amount below threshold.")]
        public void AvailableBalanceRule_WhenMinAmountRequiredTrue_ThenDecisionBasedOnMaxAmountLess(
            decimal transactionAmount,
            decimal originalCardBaseAmount,
            decimal maxAmountLess,
            decimal availableBalance,
            bool expectedApproved,
            JITFundingDeclineReason expectedDeclineReason)
        {
            //Arrange
            var rule = new AvailableBalanceRule();
            var mockRequest = new JITFundingRequest
            {
                IsMinAmountRequired = true,
                TransactionAmount = transactionAmount
            };
            var mockVCard = new VCard
            {
                VCardStatusId = CardStatus.Open,
                OriginalCardBaseAmount = originalCardBaseAmount,
                MaxAmountLess = maxAmountLess,
                AvailableBalance = availableBalance
            };

            //Act
            var actual = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.AreEqual(expectedApproved, actual.Approved);
            Assert.AreEqual(expectedDeclineReason, actual.DeclineReason);
        }

        #endregion AvailableBalanceRule Tests

        #region FundedRule Tests

        [Test]
        public void FundedRule_Declines_LeaseStatus_AlreadyFunded()
        {
            //Arrange
            var rule = new FundedRule();
            var mockRequest = new JITFundingRequest { LeaseStatus = "FUNDED", IsMinAmountRequired = true};
            var mockVCard = new VCard { VCardStatusId = CardStatus.Authorized };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsFalse(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.LeaseAlreadyFunded, decision.DeclineReason);
        }

        [Test]
        public void FundedRule_Declines_LeaseStatus_VCardNotAuthorized()
        {
            //Arrange
            var rule = new FundedRule();
            var mockRequest = new JITFundingRequest { LeaseStatus = "FUNDED", IsMinAmountRequired = false };
            var mockVCard = new VCard { VCardStatusId = CardStatus.Cancelled };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsTrue(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.None, decision.DeclineReason);
        }

        [Test]
        public void FundedRule_Declines_LeaseStatus_IsMinAmountRequiredIsFalse()
        {
            //Arrange
            var rule = new FundedRule();
            var mockRequest = new JITFundingRequest { LeaseStatus = "FUNDED", IsMinAmountRequired = false };
            var mockVCard = new VCard { VCardStatusId = CardStatus.Authorized };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsTrue(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.None, decision.DeclineReason);
        }

        [TestCase("abcdef", Description = "LeaseStatus any value other than 'FUNDED'")]
        [TestCase("")]
        [TestCase(null)]
        public void FundedRule_Approves_LeaseStatus_NotFunded(string leaseStatus)
        {
            //Arrange
            var rule = new FundedRule();
            var mockRequest = new JITFundingRequest { LeaseStatus = leaseStatus };
            var mockVCard = new VCard { VCardStatusId = CardStatus.Authorized };

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsTrue(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.None, decision.DeclineReason);
        }

        #endregion FundedRule Tests

        #region StateRule Tests

        [TestCase("", "", Description = "No state is provided")]
        [TestCase("UT", "", Description = "Only TransactionState is provided")]
        [TestCase("", "UT", Description = "Only StoreAddressState is provided")]
        public void StateRule_Declines_StateMissing(string transactionState, string storeAddressState)
        {
            //Arrange
            var rule = new StateRule();
            var mockRequest = new JITFundingRequest
            {
                UseStateValidation = true,
                TransactionState = transactionState,
                StoreAddressState = storeAddressState
            };
            var mockVCard = It.IsAny<VCard>();

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsFalse(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.StateMissing, decision.DeclineReason);
        }

        public void StateRule_Declines_StateMismatch()
        {
            //Arrange
            var rule = new StateRule();
            var mockRequest = new JITFundingRequest
            {
                UseStateValidation = true,
                TransactionState = "UT",
                StoreAddressState = "FL"
            };
            var mockVCard = It.IsAny<VCard>();

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsFalse(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.StateMismatch, decision.DeclineReason);
        }

        [TestCase("", "", Description = "No state is provided")]
        [TestCase("UT", "", Description = "Only TransactionState is provided")]
        [TestCase("", "UT", Description = "Only StoreAddressState is provided")]
        [TestCase("UT", "FL", Description = "States provided but do not match")]
        public void StateRule_NoStateValidation_Ignores_States(string transactionState, string storeAddressState)
        {
            //Arrange
            var rule = new StateRule();
            var mockRequest = new JITFundingRequest
            {
                UseStateValidation = false,
                TransactionState = transactionState,
                StoreAddressState = storeAddressState
            };
            var mockVCard = It.IsAny<VCard>();

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsTrue(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.None, decision.DeclineReason);
        }

        [Test]
        public void StateRule_Approves_StatesMatch()
        {
            //Arrange
            var rule = new StateRule();
            var mockRequest = new JITFundingRequest
            {
                UseStateValidation = true,
                TransactionState = "UT",
                StoreAddressState = "UT"
            };
            var mockVCard = It.IsAny<VCard>();

            //Act
            var decision = rule.Execute(mockRequest, mockVCard);

            //Assert
            Assert.IsTrue(decision.Approved);
            Assert.AreEqual(JITFundingDeclineReason.None, decision.DeclineReason);
        }

        #endregion StateRule Tests
    }
}
