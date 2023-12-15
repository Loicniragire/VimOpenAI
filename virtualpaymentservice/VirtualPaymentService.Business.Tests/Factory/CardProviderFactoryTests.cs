using AutoMapper;
using Moq;
using NUnit.Framework;
using ProgLeasing.System.Logging.Contract;
using System;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Factory;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Provider;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.Tests.Factory
{
    [TestFixture]
    public class CardProviderFactoryTests
    {
        #region Field
        private Mock<IServiceProvider> _mockServiceProvider;
        private CardProviderFactory _factory;
        #endregion Field

        #region Setup
        [SetUp]
        public void SetUp()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _factory = new CardProviderFactory(_mockServiceProvider.Object);
        }
        #endregion Setup

        #region GetWalletProvider Test
        [Test]
        public void GetWalletProvider_ReturnsAnIDigitalWallet_IfProviderIsValid()
        {
            // Assemble
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IMarqetaCommercialService)))
                .Returns(new Mock<IMarqetaCommercialService>().Object);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IMarqetaConsumerService)))
                .Returns(new Mock<IMarqetaConsumerService>().Object);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(ILogger<MarqetaProvider>)))
                .Returns(new Mock<ILogger<MarqetaProvider>>().Object);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(new Mock<IMapper>().Object);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(AppSettings)))
                .Returns(new Mock<AppSettings>().Object);

            // Act
            var response = _factory.GetWalletProvider(VPaymentProvider.Marqeta);

            // Assert
            Assert.IsTrue(response is IDigitalWalletProvider);
        }

        [Test]
        public void GetWalletProvider_ThrowsException_IfProviderIsNotValid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _factory.GetWalletProvider(VPaymentProvider.Wex));
        }
        #endregion GetWalletProvider Test

        #region GetCardProvider Test
        [Test]
        public void GetCardProvider_WhenCardProviderSupported_ThenProviderReturned()
        {
            // Assemble
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IMarqetaCommercialService)))
                .Returns(new Mock<IMarqetaCommercialService>().Object);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IMarqetaConsumerService)))
                .Returns(new Mock<IMarqetaConsumerService>().Object);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(ILogger<MarqetaProvider>)))
                .Returns(new Mock<ILogger<MarqetaProvider>>().Object);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IMapper)))
                .Returns(new Mock<IMapper>().Object);
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(AppSettings)))
                .Returns(new Mock<AppSettings>().Object);

            // Act
            var response = _factory.GetCardProvider(VirtualCardProviderNetwork.Marqeta);

            // Assert
            Assert.IsTrue(response is ICardProvider);
        }

        [Test]
        public void GetCardProvider_WhenCardProviderNotSupported_ThenExceptionReturned()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _factory.GetCardProvider(0));
        }

        #endregion GetCardProvider Test
    }
}