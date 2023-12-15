namespace VirtualPaymentService.Tests.HealthChecks
{
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using VirtualPaymentService.Business.Interface;
    using VirtualPaymentService.HealthChecks;
    using VirtualPaymentService.Model.Enums;
    using VirtualPaymentService.Model.Requests;
    using VirtualPaymentService.Model.Responses;

    public interface IMockVCardProvider : ICardProvider { }

    public class MockVCardProviderTrue : IMockVCardProvider
    {
        public VPaymentProvider Provider => VPaymentProvider.QA;

        public Task<bool> IsHealthyAsync()
        {
            return Task.FromResult(true);
        }

        public Task<VirtualCardResponse> CreateCardAsync(VirtualCardRequest request, ProductType productType)
        {
            return Task.FromResult(new VirtualCardResponse());
        }

        public Task<bool> CancelCardAsync(CancelVCardRequest vCard, ProductType productType)
        {
            return Task.FromResult(true);
        }

        public Task<CardUserResponse> GetCardUserAsync(string userToken, ProductType productType)
        {
            return Task.FromResult(new CardUserResponse());
        }
    }

    public class MockVCardProviderFalse : IMockVCardProvider
    {
        public VPaymentProvider Provider => VPaymentProvider.Backup;

        public Task<bool> IsHealthyAsync()
        {
            return Task.FromResult(false);
        }

        public Task<VirtualCardResponse> CreateCardAsync(VirtualCardRequest request, ProductType productType)
        {
            return Task.FromResult(new VirtualCardResponse());
        }

        public Task<bool> CancelCardAsync(CancelVCardRequest vCard, ProductType productType)
        {
            return Task.FromResult(false);
        }

        public Task<CardUserResponse> GetCardUserAsync(string userToken, ProductType productType)
        {
            return Task.FromResult(new CardUserResponse());
        }
    }

    [TestFixture]
    public class VCardProviderHealthCheckTests
    {
        private VCardProviderHealthCheck _healthCheck;
        private HealthCheckContext _healthContext;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void SetUp()
        {
            _serviceProvider = new Mock<IServiceProvider>().Object;

            _healthContext = new HealthCheckContext
            {
                Registration = new HealthCheckRegistration("FakeValue", new Mock<IHealthCheck>().Object, new HealthStatus?(), It.IsAny<string[]>())
            };
        }

        [Test]
        [TestCase(typeof(MockVCardProviderFalse))]
        [TestCase(typeof(MockVCardProviderFalse), typeof(MockVCardProviderTrue), Description = "With only some failed ping request")]
        public async Task Returns_Unhealthy_HealthCheckResult_WithProperlyFormattedResults(params Type[] cardProviders)
        {
            // Assemble
            _healthCheck = new VCardProviderHealthCheck(_serviceProvider, cardProviders);

            // Act
            HealthCheckResult result = await _healthCheck.CheckHealthAsync(_healthContext, CancellationToken.None);

            // Assert
            var containsTrueProvider = result.Data.TryGetValue("QA communication success", out var truePing);
            if (containsTrueProvider)
            {
                Assert.AreEqual(true, truePing);
            }

            Assert.True(result.Data.TryGetValue("Backup communication success", out var falsePing));
            Assert.AreEqual(false, falsePing);
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
        }

        [Test]
        [TestCase(typeof(MockVCardProviderTrue))]
        public async Task Returns_Healthy_HealthCheckResult_WithProperlyFormattedResults(params Type[] cardProviders)
        {
            // Assemble
            _healthCheck = new VCardProviderHealthCheck(_serviceProvider, cardProviders);

            // Act
            HealthCheckResult result = await _healthCheck.CheckHealthAsync(_healthContext, CancellationToken.None);

            // Assert
            Assert.True(result.Data.TryGetValue("QA communication success", out var truePing));
            Assert.AreEqual(true, truePing);
            Assert.AreEqual(HealthStatus.Healthy, result.Status);
        }
    }
}