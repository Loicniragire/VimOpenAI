using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Common.AutoMapper;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Factory.Interface;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Provider;
using VirtualPaymentService.Business.Tests.Service.Helpers;
using VirtualPaymentService.Data.DynamicQueries;
using VirtualPaymentService.Data.Facade;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Tests.Provider
{
    [TestFixture]
    public class VirtualCardProviderTests
    {
        #region Field
        private Mock<ICardProviderFactory> _mockProviderFactory;
        private Mock<IVirtualPaymentRepository> _mockVPayRepo;
        private Mock<IDigitalWalletProvider> _mockDigitalWalletProvider;
        private Mock<ICardProvider> _mockCardProvider;
        private VirtualCardProvider _provider;
        private AppSettings _appsettings;
        #endregion Field

        #region Setup
        [SetUp]
        public void SetUp()
        {
            _mockProviderFactory = new Mock<ICardProviderFactory>();
            _mockVPayRepo = new Mock<IVirtualPaymentRepository>();
            _mockDigitalWalletProvider = new Mock<IDigitalWalletProvider>();
            _mockCardProvider = new Mock<ICardProvider>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DigitalWalletProfile>();
                cfg.AddProfile<VirtualCardProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            // Gets appsettings.json, found in the test project.
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Bind the appsettings.
            _appsettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(_appsettings);

            _provider = new VirtualCardProvider(_mockProviderFactory.Object,
                                                _mockVPayRepo.Object,
                                                mapper,
                                                _appsettings);
        }
        #endregion Setup

        #region CancelVCardAsync Tests
        [Test]
        public async Task Given_CancelVCardAsync_When_ProviderCancelFails_Then_ReturnFalse()
        {
            // Arrange
            _mockCardProvider
                .Setup(p => p.CancelCardAsync(It.IsAny<CancelVCardRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(false);

            _mockProviderFactory
                .Setup(fac => fac.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    {
                        new ProviderProductType
                        {
                            ProviderProductTypeId = 1,
                            ProviderId = VirtualCardProviderNetwork.Marqeta,
                            ProductTypeId = ProductType.Commercial,
                            IsActive = true,
                            IsDefault = true
                        }
                    }
                });

            // Act
            var isSuccess = await _provider.CancelVCardAsync(new CancelVCardRequest());

            // Assert
            Assert.IsFalse(isSuccess);
        }

        [Test]
        public void Given_CancelVCardAsync_When_RepoThrows_Then_MethodThrows()
        {
            // Arrange
            _mockCardProvider
                .Setup(p => p.CancelCardAsync(It.IsAny<CancelVCardRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(true);

            _mockProviderFactory
                .Setup(fac => fac.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            _mockVPayRepo
                .Setup(repo => repo.CancelVCardAsync(It.IsAny<CancelVCardRequest>()))
                .ThrowsAsync(new Exception());

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    {
                        new ProviderProductType
                        {
                            ProviderProductTypeId = 1,
                            ProviderId = VirtualCardProviderNetwork.Marqeta,
                            ProductTypeId = ProductType.Commercial,
                            IsActive = true,
                            IsDefault = true
                        }
                    }
                });

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _provider.CancelVCardAsync(new CancelVCardRequest()));
        }

        [Test]
        public async Task Given_CancelVCardAsync_When_ProviderAndRepoSuccess_Then_ReturnsTrue()
        {
            // Arrange
            _mockCardProvider
                .Setup(p => p.CancelCardAsync(It.IsAny<CancelVCardRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(true);

            _mockProviderFactory
                .Setup(fac => fac.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            _mockVPayRepo
                .Setup(repo => repo.CancelVCardAsync(It.IsAny<CancelVCardRequest>()))
                .ReturnsAsync(1);

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    {
                        new ProviderProductType
                        {
                            ProviderProductTypeId = 1,
                            ProviderId = VirtualCardProviderNetwork.Marqeta,
                            ProductTypeId = ProductType.Commercial,
                            IsActive = true,
                            IsDefault = true
                        }
                    }
                });

            // Act
            var isSuccess = await _provider.CancelVCardAsync(new CancelVCardRequest());

            // Assert
            Assert.IsTrue(isSuccess);
        }

        /// <summary>
        /// Test written to call VCardProvider.CancelVCardAsync, but could be applied to any method in class <see cref="VCardProvider"/>,
        /// that reads results of a vcard's <see cref="VCardProviderProductType"/>, (Or when not exists, the default: <see cref="ProviderProductType"/>).
        /// However, since this test specifically validates functionality of private method VCardProvider.GetProductTypeByCardTokenAsync,
        /// applying duplicate tests to all methods that call VCardProvider.GetProductTypeByCardTokenAsync, seems like overkill.
        /// </summary>
        [Test]
        public async Task Given_CancelVCardAsync_When_NoProductFoundAssociatedWithCard_ThenUseProviderDefault()
        {
            // Arrange
            var expectedDefaultProductType = ProductType.Commercial;
            var testReferenceId = "TestCardToken";

            _mockCardProvider
                .Setup(p => p.CancelCardAsync(It.IsAny<CancelVCardRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(true);

            _mockProviderFactory
                .Setup(fac => fac.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            _mockVPayRepo
                .Setup(repo => repo.CancelVCardAsync(It.IsAny<CancelVCardRequest>()))
                .ReturnsAsync(1);

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = true,
                        IsDefault = true
                    },
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 2,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Consumer,
                        IsActive = true,
                        IsDefault = false
                    }
                });

            // Act
            var isSuccess = await _provider.CancelVCardAsync(new CancelVCardRequest { ReferenceId = testReferenceId });

            // Assert
            Assert.IsTrue(isSuccess);
            _mockVPayRepo.Verify(
                repo => repo.GetVCardProviderProductTypeAsync(It.Is<string>(r => r == testReferenceId)), Times.Once);
            _mockVPayRepo.Verify(
                repo => repo.GetProviderProductTypeAsync(), Times.Once);
            _mockCardProvider.Verify(
                provider => provider.CancelCardAsync(It.IsAny<CancelVCardRequest>(),
                    It.Is<ProductType>(p => p == expectedDefaultProductType)),
                Times.Once);
        }

        /// <summary>
        /// Test written to call VCardProvider.CancelVCardAsync, but could be applied to any method in class <see cref="VCardProvider"/>,
        /// that reads results of a vcard's <see cref="VCardProviderProductType"/>, (Or when not exists, the default: <see cref="ProviderProductType"/>).
        /// However, since this test specifically validates functionality of private method VCardProvider.GetProductTypeByCardTokenAsync,
        /// applying duplicate tests to all methods that call VCardProvider.GetProductTypeByCardTokenAsync, seems like overkill.
        /// </summary>
        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task Given_CancelVCardAsync_When_VCardProviderTypeFoundAssociatedWithCard_ThenUseProviderDefault(
            ProductType expectedVCardProviderProductType)
        {
            // Arrange
            var testReferenceId = "TestCardToken";

            _mockCardProvider
                .Setup(p => p.CancelCardAsync(It.IsAny<CancelVCardRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(true);

            _mockProviderFactory
                .Setup(fac => fac.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            _mockVPayRepo
                .Setup(repo => repo.CancelVCardAsync(It.IsAny<CancelVCardRequest>()))
                .ReturnsAsync(1);

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = true,
                        IsDefault = true
                    },
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 2,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Consumer,
                        IsActive = true,
                        IsDefault = false
                    }
                });

            _mockVPayRepo
                .Setup(repo => repo.GetVCardProviderProductTypeAsync(It.IsAny<string>()))
                .ReturnsAsync(new VCardProviderProductType
                {
                    ProviderId = VirtualCardProviderNetwork.Marqeta,
                    ProductTypeId = expectedVCardProviderProductType
                });

            // Act
            var isSuccess = await _provider.CancelVCardAsync(new CancelVCardRequest() { ReferenceId = testReferenceId });

            // Assert
            Assert.IsTrue(isSuccess);
            _mockVPayRepo.Verify(
                repo => repo.GetVCardProviderProductTypeAsync(It.Is<string>(r => r == testReferenceId)), Times.Once);
            _mockVPayRepo.Verify(
                repo => repo.GetProviderProductTypeAsync(), Times.Never);
            _mockCardProvider.Verify(
                provider => provider.CancelCardAsync(It.IsAny<CancelVCardRequest>(),
                    It.Is<ProductType>(p => p == expectedVCardProviderProductType)),
                Times.Once);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void Given_CancelVCardAsync_When_NoActiveDefaultProviderTypeFound_ThenThrowsNotSupportedException(bool isActive, bool isDefault)
        {
            // Arrange
            var testReferenceId = "TestCardToken";

            _mockCardProvider
                .Setup(p => p.CancelCardAsync(It.IsAny<CancelVCardRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(true);

            _mockProviderFactory
                .Setup(fac => fac.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            _mockVPayRepo
                .Setup(repo => repo.CancelVCardAsync(It.IsAny<CancelVCardRequest>()))
                .ReturnsAsync(1);

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = isActive,
                        IsDefault = isDefault
                    }
                });

            // Act
            // Assert
            Assert.ThrowsAsync<NotSupportedException>(async () => await _provider.CancelVCardAsync(new CancelVCardRequest() { ReferenceId = testReferenceId }));
        }

        [TestCase()]
        public void Given_CancelVCardAsync_When_NullProviderTypeFound_ThenThrowsNotSupportedException()
        {
            // Arrange
            var testReferenceId = "TestCardToken";

            _mockCardProvider
                .Setup(p => p.CancelCardAsync(It.IsAny<CancelVCardRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(true);

            _mockProviderFactory
                .Setup(fac => fac.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            _mockVPayRepo
                .Setup(repo => repo.CancelVCardAsync(It.IsAny<CancelVCardRequest>()))
                .ReturnsAsync(1);

            // Act
            // Assert
            Assert.ThrowsAsync<NotSupportedException>(async () => await _provider.CancelVCardAsync(new CancelVCardRequest() { ReferenceId = testReferenceId }));
        }

        #endregion CancelVCardAsync Tests

        #region GetApplePayTokenizationDataAsync Test 
        [Test]
        public async Task GetApplePayTokenizationDataAsync_ReturnsDigitalWalletProviderResponse()
        {
            // Assemble
            var expected = new ApplePayTokenizationResponse
            {
                CardToken = "My Card Token",
                ActivationData = "Active"
            };
            _mockDigitalWalletProvider
                .Setup(p => p.GetApplePayTokenizationDataAsync(It.IsAny<ApplePayProvisioningData>(), It.IsAny<ProductType>()))
                .ReturnsAsync(expected);
            _mockProviderFactory
                .Setup(factory => factory.GetWalletProvider(It.IsAny<VPaymentProvider>()))
                .Returns(_mockDigitalWalletProvider.Object);
            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    {
                        new ProviderProductType
                        {
                            ProviderProductTypeId = 1,
                            ProviderId = VirtualCardProviderNetwork.Marqeta,
                            ProductTypeId = ProductType.Commercial,
                            IsActive = true,
                            IsDefault = true
                        }
                    }
                });

            // Act
            var response = await _provider.GetApplePayTokenizationDataAsync(new ApplePayTokenizationRequest() { Data = new ApplePayProvisioningData() });

            // Assert
            Assert.NotNull(response.ActivationData);
            Assert.AreEqual(expected.CardToken, response.CardToken);
        }
        #endregion GetApplePayTokenizationDataAsync Test

        #region GetGooglePayTokenizationDataAsync Test 
        [Test]
        public async Task GetGooglePayTokenizationDataAsync_ReturnsValid_TokenizationResponse()
        {
            // Assemble
            var expected = new GooglePayTokenizationResponse
            {
                CardToken = "My Card Token",
                PushTokenizeRequestData = new GooglePayTokenizationData
                {
                    OpaquePaymentCard = "eyJraWQiOiIxVjMwT1ZCUTNUMjRZMVFBVFRRUza",
                    UserAddress = new GooglePayUserAddress
                    {
                        PostalCode = "04312"
                    }
                }
            };

            _mockDigitalWalletProvider
                .Setup(p => p.GetGooglePayTokenizationDataAsync(It.IsAny<GooglePayProvisioningData>(), It.IsAny<ProductType>()))
                .ReturnsAsync(expected);
            _mockProviderFactory
                .Setup(factory => factory.GetWalletProvider(It.IsAny<VPaymentProvider>()))
                .Returns(_mockDigitalWalletProvider.Object);
            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    {
                        new ProviderProductType
                        {
                            ProviderProductTypeId = 1,
                            ProviderId = VirtualCardProviderNetwork.Marqeta,
                            ProductTypeId = ProductType.Commercial,
                            IsActive = true,
                            IsDefault = true
                        }
                    }
                });

            // Act
            var response = await _provider.GetGooglePayTokenizationDataAsync(new GooglePayTokenizationRequest() { Data = new GooglePayProvisioningData() });

            // Assert
            Assert.IsInstanceOf(typeof(GooglePayTokenizationResponse), response);

            Assert.NotNull(response);
            Assert.AreEqual(expected.CardToken, response.CardToken);

            Assert.NotNull(response.PushTokenizeRequestData);
            Assert.AreEqual(expected.PushTokenizeRequestData.OpaquePaymentCard, response.PushTokenizeRequestData.OpaquePaymentCard);

            Assert.NotNull(response.PushTokenizeRequestData.UserAddress);
            Assert.AreEqual(expected.PushTokenizeRequestData.UserAddress.PostalCode, response.PushTokenizeRequestData.UserAddress.PostalCode);
        }
        #endregion GetGooglePayTokenizationDataAsync Test

        #region GetVCardByLease Test
        [Test]
        public async Task GetVCardByLease_ReturnsVCard()
        {
            // Assemble
            var referenceId = "My id";
            var expected = new List<VCard>()
                {
                    new VCard()
                    {
                        ReferenceId = referenceId,
                        ProductTypeId = ProductType.Commercial,
                    }
                };

            _mockVPayRepo
                .Setup(repo => repo.GetVCardsByLeaseAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<VCard>()
                {
                    new VCard()
                    {
                        ReferenceId = referenceId,
                        ProductTypeId = null,
                    }
                });

            // Act
            var response = await _provider.GetVCardsByLeaseAsync(123);

            // Assert
            response.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetVCardByLease_WhenCardsLocated_ThenProductTypeMappingReturned()
        {
            // Assemble
            var expected = new List<VCard>()
                {
                    new VCard()
                    {
                        ReferenceId = "ID for Marqeta Commercial",
                        ProductTypeId = ProductType.Commercial,
                    },
                    new VCard()
                    {
                        ReferenceId = "ID for Marqeta Consumer",
                        ProductTypeId = ProductType.Consumer,
                    },
                    new VCard()
                    {
                        ReferenceId = "ID for Wex",
                        ProductTypeId = ProductType.Commercial,
                    }
                };

            _mockVPayRepo
                .Setup(repo => repo.GetVCardsByLeaseAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<VCard>()
                {
                    new VCard()
                    {
                        ReferenceId = expected[0].ReferenceId,
                        // Call to Progressive DB will not return this value, is returned as null.
                        ProductTypeId = null
                    },
                    new VCard()
                    {
                        ReferenceId = expected[1].ReferenceId,
                        // Call to Progressive DB will not return this value, is returned as null.
                        ProductTypeId = null
                    },
                    new VCard()
                    {
                        ReferenceId = expected[2].ReferenceId,
                        // Call to Progressive DB will not return this value, is returned as null.
                        ProductTypeId = null
                    }
                });

            _mockVPayRepo
                .Setup(repo => repo.GetVCardProviderProductTypeAsync(It.Is<string>(r => r == expected[0].ReferenceId)))
                .ReturnsAsync(new VCardProviderProductType()
                {
                    ProductTypeId = ProductType.Commercial
                });

            _mockVPayRepo
                .Setup(repo => repo.GetVCardProviderProductTypeAsync(It.Is<string>(r => r == expected[1].ReferenceId)))
                .ReturnsAsync(new VCardProviderProductType()
                {
                    ProductTypeId = ProductType.Consumer
                });

            // Act
            var response = await _provider.GetVCardsByLeaseAsync(123);

            // Assert
            response.Should().BeEquivalentTo(expected);
        }
        #endregion GetVCardByLease Test

        #region GetDigitalWalletTokensByVCardAsync Tests
        [Test]
        public async Task GetDigitalWalletTokensByVCardAsync_ReturnsDigitalWalletTokenResponse()
        {
            // Assemble
            var expected = new DigitalWalletTokenResponse
            {
                DigitalWalletTokens = new List<DigitalWalletToken>()
            };

            _mockDigitalWalletProvider
                .Setup(p => p.GetDigitalWalletTokensByVCardAsync(It.IsAny<string>(), It.IsAny<ProductType>()))
                .ReturnsAsync(expected);
            _mockProviderFactory
                .Setup(factory => factory.GetWalletProvider(It.IsAny<VPaymentProvider>()))
                .Returns(_mockDigitalWalletProvider.Object);
            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    {
                        new ProviderProductType
                        {
                            ProviderProductTypeId = 1,
                            ProviderId = VirtualCardProviderNetwork.Marqeta,
                            ProductTypeId = ProductType.Commercial,
                            IsActive = true,
                            IsDefault = true
                        }
                    }
                });

            // Act
            var response = await _provider.GetDigitalWalletTokensByVCardAsync("Card Token");

            // Assert
            Assert.AreEqual(expected, response);
        }
        #endregion GetDigitalWalletTokensByVCardAsync

        #region TransitionDigitalWalletTokenAsync Tests
        [Test]
        public async Task TransitionDigitalWalletTokenAsync_ReturnsDigitalWalletTokenResponse()
        {
            // Assemble
            var expected = new DigitalWalletTokenTransitionResponse
            {
                WalletTransitionToken = "test_transition_token",
                WalletTransitionState = DigitalWalletTokenStatus.Green,
                WalletTransitionType = "state.activated",
                FulfillmentStatus = "PROVISIONED"
            };

            _mockDigitalWalletProvider
                .Setup(p => p.TransitionDigitalWalletTokenAsync(It.IsAny<DigitalWalletTokenTransitionRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(expected);
            _mockProviderFactory
                .Setup(factory => factory.GetWalletProvider(It.IsAny<VPaymentProvider>()))
                .Returns(_mockDigitalWalletProvider.Object);
            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    {
                        new ProviderProductType
                        {
                            ProviderProductTypeId = 1,
                            ProviderId = VirtualCardProviderNetwork.Marqeta,
                            ProductTypeId = ProductType.Commercial,
                            IsActive = true,
                            IsDefault = true
                        }
                    }
                });

            // Act
            var response = await _provider.TransitionDigitalWalletTokenAsync(new DigitalWalletTokenTransitionRequest
            { DigitalWalletToken = new DigitalWalletToken { CardToken = "test_card_token" } });

            // Assert
            Assert.AreEqual(expected, response);
        }
        #endregion TransitionDigitalWalletTokenAsync Tests

        #region UpdateCardUserAsync Tests
        [Test]
        public async Task UpdateCardUserAsync_WhenUserUpdatedAtProviderForAllCardProducts_ThenUserResponseReturned()
        {
            // Assemble
            var expected = new CardUserUpdateResponse()
            {
                PhoneNumber = "+18015551212",
                Active = true,
                CreatedTime = DateTime.Now,
                LastModifiedTime = DateTime.Now
            };

            _mockDigitalWalletProvider
                .Setup(p => p.UpdateCardUserAsync(It.IsAny<string>(), It.IsAny<CardUserUpdateRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(expected);
            _mockProviderFactory
                .Setup(factory => factory.GetWalletProvider(It.IsAny<VPaymentProvider>()))
                .Returns(_mockDigitalWalletProvider.Object);
            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = true,
                        IsDefault = true
                    },
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 2,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Consumer,
                        IsActive = true,
                        IsDefault = false
                    }
                });

            // Act
            var response = await _provider.UpdateCardUserAsync("User Token", new CardUserUpdateRequest());

            // Assert
            Assert.AreEqual(expected, response);
        }

        [TestCase(ProductType.Commercial)]
        [TestCase(ProductType.Consumer)]
        public async Task UpdateCardUserAsync_WhenUserUpdatedAtProviderCardProduct_ButFailsOnAnother_ThenUserResponseReturned(ProductType failProductType)
        {
            // Assemble
            var expected = new CardUserUpdateResponse()
            {
                PhoneNumber = "+18015551212",
                Active = true,
                CreatedTime = DateTime.Now,
                LastModifiedTime = DateTime.Now
            };

            _mockDigitalWalletProvider
                .Setup(p => p.UpdateCardUserAsync(It.IsAny<string>(), It.IsAny<CardUserUpdateRequest>(), It.Is<ProductType>(p => p != failProductType)))
                .ReturnsAsync(expected);
            _mockDigitalWalletProvider
                .Setup(p => p.UpdateCardUserAsync(It.IsAny<string>(), It.IsAny<CardUserUpdateRequest>(), It.Is<ProductType>(p => p == failProductType)))
                .ThrowsAsync(new HttpRequestException());
            _mockProviderFactory
                .Setup(factory => factory.GetWalletProvider(It.IsAny<VPaymentProvider>()))
                .Returns(_mockDigitalWalletProvider.Object);
            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = true,
                        IsDefault = true
                    },
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 2,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Consumer,
                        IsActive = true,
                        IsDefault = false
                    }
                });

            // Act
            var response = await _provider.UpdateCardUserAsync("User Token", new CardUserUpdateRequest());

            // Assert
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void UpdateCardUserAsync_WhenUserUpdateFailsForAllProviderCardProducts_ThenThrowsHttpRequestException()
        {
            // Assemble
            _mockDigitalWalletProvider
                .Setup(p => p.UpdateCardUserAsync(It.IsAny<string>(), It.IsAny<CardUserUpdateRequest>(), It.IsAny<ProductType>()))
                .ThrowsAsync(new HttpRequestException());
            _mockProviderFactory
                .Setup(factory => factory.GetWalletProvider(It.IsAny<VPaymentProvider>()))
                .Returns(_mockDigitalWalletProvider.Object);
            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = true,
                        IsDefault = true
                    },
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 2,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Consumer,
                        IsActive = true,
                        IsDefault = false
                    }
                });

            // Act
            // Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _provider.UpdateCardUserAsync("User Token", new CardUserUpdateRequest()));
        }

        [Test]
        public void UpdateCardUserAsync_WhenUserUpdateFailsForAllProviderCardProducts_ThenThrowsHttpRequestExceptionWithInnerEx()
        {

            // Assemble
            _mockDigitalWalletProvider
                .Setup(p => p.UpdateCardUserAsync(It.IsAny<string>(), It.IsAny<CardUserUpdateRequest>(), It.IsAny<ProductType>()))
                .ThrowsAsync(GenerateInnerException());
            _mockProviderFactory
                .Setup(factory => factory.GetWalletProvider(It.IsAny<VPaymentProvider>()))
                .Returns(_mockDigitalWalletProvider.Object);
            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = true,
                        IsDefault = true
                    },
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 2,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Consumer,
                        IsActive = true,
                        IsDefault = false
                    }
                });

            // Act
            // Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _provider.UpdateCardUserAsync("User Token", new CardUserUpdateRequest()));
        }

        [Test]
        public void UpdateCardUserAsync_WhenUserUpdateFailsWithGeneralException_ThenThrowsException()
        {

            // Assemble
            _mockDigitalWalletProvider
                .Setup(p => p.UpdateCardUserAsync(It.IsAny<string>(), It.IsAny<CardUserUpdateRequest>(), It.IsAny<ProductType>()))
                .ThrowsAsync(new Exception());
            _mockProviderFactory
                .Setup(factory => factory.GetWalletProvider(It.IsAny<VPaymentProvider>()))
                .Returns(_mockDigitalWalletProvider.Object);
            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = true,
                        IsDefault = true
                    },
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 2,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Consumer,
                        IsActive = true,
                        IsDefault = false
                    }
                });

            // Act
            // Assert
            Assert.ThrowsAsync<Exception>(async () => await _provider.UpdateCardUserAsync("User Token", new CardUserUpdateRequest()));
        }

        private HttpRequestException GenerateInnerException()
        {
            try
            {
                throw new HttpRequestException();
            }
            catch (HttpRequestException ex)
            {
                return ex;
            }
        }
        #endregion UpdateCardUserAsync Tests

        #region GetVCardProvidersAsync Tests
        [Test]
        public async Task Given_GetVCardProvidersAsync_When_CalledWithFalse_Then_ReturnsAll()
        {
            // Arrange
            var expected = new List<VCardProvider>
            {
                {
                    new VCardProvider
                    {
                        ProviderId = 1,
                        ProviderName = "Fake Provider",
                        AuthExpireDays = 12,
                        IsActive = true,
                        BusinessRank = 3,
                        CreditLimit = 123456.00M,
                        CreditLimitCutoffPercentage = 0.9M,
                        CreditLimitReached = false
                    }
                },
                {
                    new VCardProvider
                    {
                        ProviderId = 2,
                        ProviderName = "Fake Provider two",
                        AuthExpireDays = 12,
                        IsActive = false,
                        BusinessRank = 3,
                        CreditLimit = 123456.00M,
                        CreditLimitCutoffPercentage = 0.9M,
                        CreditLimitReached = false
                    }
                }
            };

            _mockVPayRepo.Setup(repo => repo.GetVCardProvidersAsync())
                .ReturnsAsync(expected);

            _mockVPayRepo.Setup(repo => repo.GetAuthTotalByProviderIdAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new VCardProviderCreditLimit { TotalAuthAmount = 0 });

            // Act
            var actual = await _provider.GetVCardProvidersAsync(false);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Given_GetVCardProvidersAsync_When_CalledWithTrue_Then_ReturnsFilteredResult()
        {
            // Arrange
            var expected = new List<VCardProvider>
            {
                {
                    new VCardProvider
                    {
                        ProviderId = 1,
                        ProviderName = "Fake Provider",
                        AuthExpireDays = 12,
                        IsActive = true,
                        BusinessRank = 3,
                        CreditLimit = 123456.00M,
                        CreditLimitCutoffPercentage = 0.9M,
                        CreditLimitReached = false
                    }
                },
                {
                    new VCardProvider
                    {
                        ProviderId = 2,
                        ProviderName = "Fake Provider two",
                        AuthExpireDays = 12,
                        IsActive = false,
                        BusinessRank = 3,
                        CreditLimit = 123456.00M,
                        CreditLimitCutoffPercentage = 0.9M,
                        CreditLimitReached = false
                    }
                }
            };

            _mockVPayRepo.Setup(repo => repo.GetVCardProvidersAsync())
                .ReturnsAsync(expected);

            _mockVPayRepo.Setup(repo => repo.GetAuthTotalByProviderIdAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new VCardProviderCreditLimit { TotalAuthAmount = 0 });

            // Act
            var actual = (await _provider.GetVCardProvidersAsync(true)).ToList();

            // Assert
            actual[0].Should().BeEquivalentTo(expected[0]);
        }

        [Test]
        public async Task Given_GetVCardProvidersAsync_When_AuthTotals_Are_Exact_Threshold_Amts_Then_CreditLimitReached_False()
        {
            // Arrange
            var expected = new List<VCardProvider>
            {
                {
                    new VCardProvider
                    {
                        ProviderId = 1,
                        ProviderName = "Fake Provider",
                        AuthExpireDays = 12,
                        IsActive = true,
                        BusinessRank = 3,
                        CreditLimit = 100000.00M,
                        CreditLimitCutoffPercentage = 1.0M,
                        CreditLimitReached = false
                    }
                },
                {
                    new VCardProvider
                    {
                        ProviderId = 2,
                        ProviderName = "Fake Provider two",
                        AuthExpireDays = 12,
                        IsActive = true,
                        BusinessRank = 3,
                        CreditLimit = 200000.00M,
                        CreditLimitCutoffPercentage = 0.5M,
                        CreditLimitReached = false
                    }
                }
            };

            _mockVPayRepo.Setup(repo => repo.GetVCardProvidersAsync())
                .ReturnsAsync(expected);

            // Exact threshold amount of auth totals
            _mockVPayRepo.Setup(repo => repo.GetAuthTotalByProviderIdAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new VCardProviderCreditLimit { TotalAuthAmount = 100000M });

            // Act
            var actual = await _provider.GetVCardProvidersAsync(false);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Given_GetVCardProvidersAsync_When_AuthTotals_Are_Over_Threshold_Amts_Then_CreditLimitReached_True()
        {
            // Arrange
            var expected = new List<VCardProvider>
            {
                {
                    new VCardProvider
                    {
                        ProviderId = 1,
                        ProviderName = "Fake Provider",
                        AuthExpireDays = 12,
                        IsActive = true,
                        BusinessRank = 3,
                        CreditLimit = 100000.00M,
                        CreditLimitCutoffPercentage = 1.0M,
                        CreditLimitReached = true
                    }
                },
                {
                    new VCardProvider
                    {
                        ProviderId = 2,
                        ProviderName = "Fake Provider two",
                        AuthExpireDays = 12,
                        IsActive = true,
                        BusinessRank = 3,
                        CreditLimit = 200000.00M,
                        CreditLimitCutoffPercentage = 0.5M,
                        CreditLimitReached = true
                    }
                }
            };

            _mockVPayRepo.Setup(repo => repo.GetVCardProvidersAsync())
                .ReturnsAsync(expected);

            // 1 Dollar over threshold auth totals
            _mockVPayRepo.Setup(repo => repo.GetAuthTotalByProviderIdAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new VCardProviderCreditLimit { TotalAuthAmount = 100001M });

            // Act
            var actual = await _provider.GetVCardProvidersAsync(false);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Given_GetVCardProvidersAsync_When_CreditLimit_GTZero_Then_Only_Will_CreditLimitReached_Be_Calculated()
        {
            // Arrange
            var expected = new List<VCardProvider>
            {
                {
                    new VCardProvider
                    {
                        ProviderId = 1,
                        ProviderName = "Fake Provider",
                        AuthExpireDays = 12,
                        IsActive = true,
                        BusinessRank = 3,
                        CreditLimit = 0,
                        CreditLimitCutoffPercentage = 1.0M,
                        CreditLimitReached = false
                    }
                },
                {
                    new VCardProvider
                    {
                        ProviderId = 2,
                        ProviderName = "Fake Provider two",
                        AuthExpireDays = 12,
                        IsActive = true,
                        BusinessRank = 3,
                        CreditLimit = 105.00M,
                        CreditLimitCutoffPercentage = 0.1M,
                        CreditLimitReached = true
                    }
                }
            };

            _mockVPayRepo.Setup(repo => repo.GetVCardProvidersAsync())
                .ReturnsAsync(expected);

            // CreditLimit reached always false when CreditLimit is $0, as calculations are not performed
            _mockVPayRepo.Setup(repo => repo.GetAuthTotalByProviderIdAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new VCardProviderCreditLimit { TotalAuthAmount = 10.51M });

            // Act
            var actual = await _provider.GetVCardProvidersAsync(false);

            // Assert
            actual.Should().BeEquivalentTo(expected);

            _mockVPayRepo.Verify(
                repo => repo.GetAuthTotalByProviderIdAsync(It.IsAny<int>(), It.IsAny<DateTime>()),
                Times.Once);
        }
        #endregion GetVCardProvidersAsync Tests

        #region GetWalletBatchConfigAsync Tests
        [Test]
        public async Task GetWalletBatchConfigAsync_WhenCalled_ThenProperlyMappedResponseReturned()
        {
            // Assemble
            var expectedDate = new DateTime();
            var mockRepoResponse = new WalletBatchConfig
            {
                LastTerminationProcessedDate = expectedDate,
                WalletBatchConfigId = 1
            };
            var expected = new WalletBatchConfigResponse()
            {
                LastTerminationProcessedDate = expectedDate
            };

            _mockVPayRepo
                .Setup(repo => repo.GetWalletBatchConfigAsync())
                .ReturnsAsync(mockRepoResponse);

            // Act
            var actual = await _provider.GetWalletBatchConfigAsync();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
        #endregion GetWalletBatchConfigAsync Tests

        #region GetCardUserAsync Tests
        [Test]
        public async Task GetCardUserAsync_WhenSearchReturnsUser_ThenUserReturned()
        {
            // Assemble
            var expected = new CardUserResponse();

            _mockCardProvider
                .Setup(p => p.GetCardUserAsync(It.IsAny<string>(), It.IsAny<ProductType>()))
                .ReturnsAsync(expected);

            _mockProviderFactory
                .Setup(fac => fac.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            // Act
            var actual = await _provider.GetCardUserAsync("123456", VirtualCardProviderNetwork.Marqeta, ProductType.Commercial);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        #endregion GetCardUserAsync Tests


        #region UpdateWalletBatchConfigAsync Tests
        [Test]
        public async Task UpdateWalletBatchConfigAsync_WhenCalled_ThenCallsRepoToUpdate_AndReturnsTask()
        {
            // Assemble
            var request = new UpdateWalletBatchConfigRequest
            {
                LastTerminationProcessedDate = DateTimeOffset.Now,
                UpdatedBy = null
            };

            _mockVPayRepo
                .Setup(repo => repo.UpdateWalletBatchConfigAsync(It.IsAny<UpdateWalletBatchConfigRequest>()))
                .Returns(Task.CompletedTask);

            // Act
            await _provider.UpdateWalletBatchConfigAsync(request);

            // Assert
            _mockVPayRepo.Verify(
                repo => repo.UpdateWalletBatchConfigAsync(It.IsAny<UpdateWalletBatchConfigRequest>()),
                Times.Once);
        }
        #endregion GetWalletBatchConfigAsync Tests

        #region GetCancelledVCardsByDateTimeAsync Tests

        [Test]
        public async Task
            GetCancelledVCardsByDateTimeAsync_WhenCalled_ThenCallsRepoWithLocalDateTime_AndReturnsCancelledVCardList()
        {
            using (new FakeLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time")))
            {
                // Assemble
                var utcTime = DateTimeOffset.UtcNow;
                var expectedDateTime = utcTime.LocalDateTime;

                var expected = new List<CancelledVCard>
                {
                    new CancelledVCard
                    {
                        VCardId = 1,
                        VCardProviderId = 6,
                        ReferenceId = "qwerty",
                        LeaseId = 1453213,
                        UpdatedDate = DateTime.Now
                    }
                };

                _mockVPayRepo
                    .Setup(repo => repo.GetCancelledVCardsByUpdatedDateAsync(expectedDateTime))
                    .ReturnsAsync(expected);

                // Act
                var actual = await _provider.GetCancelledVCardsByDateTimeAsync(utcTime);

                // Assert
                actual.Should().BeEquivalentTo(expected);
            }
        }

        #endregion GetCancelledVCardsByDateTimeAsync Tests

        #region CreateCardAsync Tests
        [Test]
        public async Task CreateCardAsync_WhenCardCreatedAtProvider_ThenCardResponseReturned()
        {
            // Assemble
            var expected = new VirtualCardResponse()
            {
                LeaseId = 123456,
                Card = new VCard()
                {
                    ReferenceId = "12345678-b32ae3e2-ef2b-4d94-8a58-acc"
                },
                User = new CardUserUpdateResponse()
            };

            _mockProviderFactory
                .Setup(factory => factory.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            _mockCardProvider
                .Setup(p => p.CreateCardAsync(It.IsAny<VirtualCardRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(expected);

            _mockVPayRepo
                .Setup(repo => repo.InsertVCard(It.IsAny<VCard>()))
                .ReturnsAsync(789);

            _mockVPayRepo
                .Setup(repo => repo.GetVCardAsync(expected.LeaseId, expected.Card.ReferenceId))
                .ReturnsAsync(expected.Card);

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    {
                        new ProviderProductType
                        {
                            ProviderProductTypeId = 1,
                            ProviderId = VirtualCardProviderNetwork.Marqeta,
                            ProductTypeId = ProductType.Commercial,
                            IsActive = true,
                            IsDefault = true
                        }
                    }
                });

            _mockVPayRepo
                .Setup(repo => repo.GetStoreProductTypeAsync(It.IsAny<int>()));

            // Act
            var response = await _provider.CreateCardAsync(new VirtualCardRequest()
            {
                LeaseId = expected.LeaseId,
                StoreId = 123456,
                VCardProviderId = VirtualCardProviderNetwork.Marqeta
            });

            // Assert
            Assert.AreEqual(expected, response);
        }

        [Test]
        public async Task CreateCardAsync_WhenStoreProductTypeMappingPresent_ThenCardCreatedWithProductMappedToStore()
        {
            // Assemble
            var expected = ProductType.Consumer;
            var storeId = 7890;
            var leaseId = 123456;

            var card = new VirtualCardResponse()
            {
                LeaseId = leaseId,
                Card = new VCard()
                {
                    ReferenceId = "12345678-b32ae3e2-ef2b-4d94-8a58-acc"
                },
                User = new CardUserUpdateResponse()
            };

            _mockProviderFactory
                .Setup(factory => factory.GetCardProvider(It.IsAny<VirtualCardProviderNetwork>()))
                .Returns(_mockCardProvider.Object);

            _mockCardProvider
                .Setup(p => p.CreateCardAsync(It.IsAny<VirtualCardRequest>(), It.IsAny<ProductType>()))
                .ReturnsAsync(card);

            _mockVPayRepo
                .Setup(repo => repo.InsertVCard(It.IsAny<VCard>()))
                .ReturnsAsync(789);

            _mockVPayRepo
                .Setup(repo => repo.GetVCardAsync(card.LeaseId, card.Card.ReferenceId))
                .ReturnsAsync(card.Card);

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {
                    {
                        new ProviderProductType
                        {
                            ProviderProductTypeId = 1,
                            ProviderId = VirtualCardProviderNetwork.Marqeta,
                            ProductTypeId = ProductType.Commercial,
                            IsActive = true,
                            IsDefault = true
                        }
                    }
                });

            _mockVPayRepo
                .Setup(repo => repo.GetStoreProductTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(new StoreProductType { StoreId = storeId, ProductTypeId = expected });

            // Act
            var actual = await _provider.CreateCardAsync(new VirtualCardRequest()
            {
                LeaseId = leaseId,
                StoreId = storeId,
                VCardProviderId = VirtualCardProviderNetwork.Marqeta
            });

            // Assert
            Assert.AreEqual(expected, actual.Card.ProductTypeId);
        }

        #endregion CreateCardAsync Tests

        #region IsRequiredUserPhoneMissingAsync Tests
        [TestCase(true, true, false, ProductType.Consumer, VirtualCardProviderNetwork.Marqeta)]
        [TestCase(false, true, true, ProductType.Consumer, VirtualCardProviderNetwork.Marqeta)]
        [TestCase(false, false, false, ProductType.Consumer, VirtualCardProviderNetwork.Marqeta)]
        [TestCase(false, false, true, ProductType.Consumer, VirtualCardProviderNetwork.Marqeta)]
        [TestCase(true, true, false, ProductType.Commercial, VirtualCardProviderNetwork.Marqeta)]
        [TestCase(false, true, true, ProductType.Commercial, VirtualCardProviderNetwork.Marqeta)]
        [TestCase(false, false, false, ProductType.Commercial, VirtualCardProviderNetwork.Marqeta)]
        [TestCase(false, false, true, ProductType.Commercial, VirtualCardProviderNetwork.Marqeta)]
        public async Task IsRequiredUserPhoneMissingAsync_WhenPhoneRequiredIsEvaluated_ThenExpectedResponseReturned(
            bool expected,
            bool setInitalPinConfigValue,
            bool phonePresentInRequest,
            ProductType productType,
            VirtualCardProviderNetwork provider)
        {
            // Assemble
            var storeId = 1234;

            // Set the initial pin config
            _appsettings.CardProviderSettings.Find(s => s.CardProvider == provider && s.ProductType == productType).SetInitalPin = setInitalPinConfigValue;

            _mockVPayRepo
                .Setup(repo => repo.GetStoreProductTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(new StoreProductType { StoreId = storeId, ProductTypeId = productType });

            // Act
            var actual = await _provider.IsRequiredUserPhoneMissingAsync(new VirtualCardRequest()
            {
                LeaseId = 12345567,
                StoreId = storeId,
                VCardProviderId = provider,
                User = phonePresentInRequest ? new CardUserUpdateRequest { PhoneNumber = "80155512121", PhoneNumberCountryCode = "1" } : null
            });

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion IsRequiredUserPhoneMissingAsync Tests

        #region GetVCardsByFilterAsync Tests
        [Test]
        public async Task GetVCardsByFilterAsync_doesit()
        {
            // Arrange
            var request = new GetVCardsRequest();
            var expected = new List<VCard>
            {
                { new VCard() }
            };

            _mockVPayRepo
                .Setup(r => r.GetFilteredVCardsAsync(It.IsAny<GetVCardsDynamicQuery>()))
                .ReturnsAsync(expected);

            // Act
            var response = await _provider.GetVCardsByFilterAsync(request);

            // Assert
            Assert.AreEqual(response.VCards, expected);
        }
        #endregion GetVCardsByFilterAsync Tests

        #region GetProductTypeAsync Tests
        [TestCase(ProductType.Consumer, VirtualCardProviderNetwork.Marqeta, true)]
        [TestCase(ProductType.Consumer, VirtualCardProviderNetwork.Marqeta, false)]
        [TestCase(ProductType.Commercial, VirtualCardProviderNetwork.Marqeta, true)]
        [TestCase(ProductType.Commercial, VirtualCardProviderNetwork.Marqeta, false)]
        public async Task GetProductTypeAsync_WhenCustomStoreProductTypeRecordExists_ThenExpectedResponseReturned(
            ProductType productTypeOfStore, VirtualCardProviderNetwork provider, bool customerInfoRequired)
        {
            // Assemble
            var storeId = 1234;
            var expected = new StoreProductType
                { StoreId = storeId, ProductTypeId = productTypeOfStore, CustomerInfoRequired = customerInfoRequired };

            _mockVPayRepo
                .Setup(repo => repo.GetStoreProductTypeAsync(It.IsAny<int>()))
                .ReturnsAsync(expected);

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {

                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = true,
                        IsDefault = true
                    },
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 2,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Consumer,
                        IsActive = true,
                        IsDefault = false
                    }
                });

            // Act
            var actual = await _provider.GetProductTypeAsync(VirtualCardProviderNetwork.Marqeta, storeId);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase(false, true, ProductType.Consumer)]
        [TestCase(true, false, ProductType.Commercial)]
        [TestCase(true, true, ProductType.Commercial)]
        [TestCase(false, false, ProductType.Null)]
        public async Task GetProductTypeAsync_WhenCustomStoreProductTypeRecordDoesNotExist_ThenDefaultProviderExpectedResponseReturned(bool isCommercialDefault, bool isConsumerDefault, ProductType expectedProductType)
        {
            // Assemble
            var storeId = 1234;

            // CustomerInfoRequired always 'false' if defaults to Provider (no custom store record)
            var expected = isCommercialDefault || isConsumerDefault
                ? new StoreProductType { StoreId = storeId, ProductTypeId = expectedProductType, CustomerInfoRequired = false }
                : null;

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ReturnsAsync(new List<ProviderProductType>
                {

                    new ProviderProductType
                    {
                        ProviderProductTypeId = 1,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Commercial,
                        IsActive = true,
                        IsDefault = isCommercialDefault
                    },
                    new ProviderProductType
                    {
                        ProviderProductTypeId = 2,
                        ProviderId = VirtualCardProviderNetwork.Marqeta,
                        ProductTypeId = ProductType.Consumer,
                        IsActive = true,
                        IsDefault = isConsumerDefault
                    }
                });

            // Act
            var actual = await _provider.GetProductTypeAsync(VirtualCardProviderNetwork.Marqeta, storeId);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GetProductTypeAsync_WhenCallToRepoForStoreProductTypeThrows_ThenMethodDoesNotCatch()
        {
            // Assemble
            var expectedEx = new Exception();

            _mockVPayRepo
                .Setup(repo => repo.GetStoreProductTypeAsync(It.IsAny<int>()))
                .ThrowsAsync(expectedEx);

            // Act & Assert
            var actualEx = Assert.ThrowsAsync<Exception>(() => _provider.GetProductTypeAsync(VirtualCardProviderNetwork.Marqeta, 1234));
            Assert.AreSame(expectedEx, actualEx);
        }

        [Test]
        public void GetProductTypeAsync_WhenCallToRepoForProviderProductTypeThrows_ThenMethodDoesNotCatch()
        {
            // Assemble
            var expectedEx = new Exception();

            _mockVPayRepo
                .Setup(repo => repo.GetProviderProductTypeAsync())
                .ThrowsAsync(expectedEx);

            // Act & Assert
            var actualEx = Assert.ThrowsAsync<Exception>(() => _provider.GetProductTypeAsync(VirtualCardProviderNetwork.Marqeta, 1234));
            Assert.AreSame(expectedEx, actualEx);
        }

        #endregion GetProductTypeAsync Tests
    }
}