using AutoMapper;
using NUnit.Framework;
using VirtualPaymentService.Business.Common.AutoMapper;

namespace VirtualPaymentService.Business.Tests.Common.AutoMapper
{
    [TestFixture]
    public class DigitalWalletProfileTests
    {
        [Test]
        public void DigitalWalletProfile_IsValid()
        {
            // Assemble
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DigitalWalletProfile>());

            // Assert
            Assert.DoesNotThrow(() => config.AssertConfigurationIsValid());
        }
    }
}