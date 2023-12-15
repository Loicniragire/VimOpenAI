using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Provider;
using VirtualPaymentService.Data;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.Tests
{
    [TestFixture]
    [Obsolete("This is a temporary solution for pulling lease/store data until VPO is able to provide it. Remove as soon as possible.")]
    public class LeaseStoreDataProviderTests
    {
        private Mock<ILeaseStoreRepository> _mockRepository;
        private LeaseStoreDataProvider _provider;

        [SetUp]
        public void Initialize()
        {
            _mockRepository = new Mock<ILeaseStoreRepository>();

            _provider = new LeaseStoreDataProvider
            (
                _mockRepository.Object
            );
        }

        [Test]
        public void LeaseStoreData_NotFound_Returns_SuccessFalse()
        {
            //Arrange
            var request = new LeaseStoreDataRequest();

            _mockRepository
                .Setup(r => r.GetDataForAuthorizationByLeaseId(It.IsAny<int>(), It.IsAny<string>()))
                .Returns<JITFundingValidation>(null);

            //Act
            var response = _provider.GetLeaseStoreData(request);

            //Assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Message);
        }

        [Test]
        public void LeaseStoreData_Found_Returns_All_Values()
        {
            //Arrange
            var request = new LeaseStoreDataRequest();
            var expected = new JITFundingValidation
            {
                InvoiceAmount = 1000.00m,
                IsMinAmountRequired = true,
                LeaseStatus = "VCard Available",
                State = "FL",
                UseStateValidation = true
            };

            _mockRepository
                .Setup(r => r.GetDataForAuthorizationByLeaseId(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(expected);

            //Act
            var actual = _provider.GetLeaseStoreData(request);

            //Assert
            Assert.IsTrue(actual.Success);
            Assert.AreEqual(expected.InvoiceAmount, actual.InvoiceAmount);
            Assert.AreEqual(expected.IsMinAmountRequired, actual.IsMinAmountRequired);
            Assert.AreEqual(expected.LeaseStatus, actual.LeaseStatus);
            Assert.AreEqual(expected.State, actual.State);
            Assert.AreEqual(expected.UseStateValidation, actual.UseStateValidation);
        }

        [Test]
        public void MarqetaState_NotFound_Returns_SuccessFalse()
        {
            //Arrange
            _mockRepository
                .Setup(r => r.GetStateForValidationByMarqetaStateId(It.IsAny<int>()))
                .Returns<string>(null);

            //Act
            var response = _provider.GetMarqetaStateAbbreviation(It.IsAny<int>());

            //Assert
            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Message);
        }

        [Test]
        public void MarqetaState_Found_Returns_Value()
        {
            //Arrange
            var expected = "UT";

            _mockRepository
                .Setup(r => r.GetStateForValidationByMarqetaStateId(It.IsAny<int>()))
                .Returns(expected);

            //Act
            var actual = _provider.GetMarqetaStateAbbreviation(It.IsAny<int>());

            //Assert
            Assert.IsTrue(actual.Success);
            Assert.AreEqual(expected, actual.MarqetaStateId);
        }

        #region GetStoreCustomFields Test
        [Test]
        public async Task GetStoreCustomFields_ReturnsVCard()
        {
            // Assemble
            var expected = new List<StoreCustomField>()
            {
                new()
                {
                    StoreId = 123,
                    ParentStoreId = 456,
                    GrandparentStoreId = 789,
                    CustomFieldName = "test tax exempt number name",
                    CustomFieldValue = "test tax exempt number value"
                }
            };


            _mockRepository
                .Setup(repo => repo.GetStoreCustomFields(It.IsAny<int>()))
                .ReturnsAsync(expected);

            // Act
            var response = await _provider.GetStoreCustomFields(123456);

            // Assert
            Assert.AreEqual(expected, response);
        }
        #endregion GetStoreCustomFields Test
    }
}
