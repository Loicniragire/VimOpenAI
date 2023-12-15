namespace VirtualPaymentService.Tests.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Moq;
    using NUnit.Framework;
    using ProgLeasing.System.Logging.Contract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using VirtualPaymentService.Controllers;
    using VirtualPaymentService.Model.Responses;

    [TestFixture]
    public class HealthControllerTests
    {
        private MockHealthCheckService _mockHealthCheckService;
        private Mock<ILogger<HealthController>> _mockLogger;
        private HealthController _healthController;

        [SetUp]
        public void SetUp()
        {
            _mockHealthCheckService = new MockHealthCheckService();
            _mockLogger = new Mock<ILogger<HealthController>>();
            _healthController = new HealthController(_mockHealthCheckService, _mockLogger.Object);
        }

        [Test]
        public async Task WhenAllHealthReportsAreHealthy_ReturnsHealthy()
        {
            var subHealthData = new Dictionary<string, object>()
            {
                { "some provider communication success", true }
            };
            var entryList = new Dictionary<string, HealthReportEntry>()
            {
                {"provider entries", new HealthReportEntry(HealthStatus.Healthy, "", new TimeSpan(), null, subHealthData) }
            };

            _mockHealthCheckService.HealthReport = new HealthReport(entryList, new TimeSpan());

            // Act
            var response = (ObjectResult)await _healthController.HealthCheckAsync();
            var healthResponse = (HealthCheckResponse)response.Value;

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
            Assert.AreEqual(HealthStatus.Healthy.ToString(), healthResponse.Status);
            Assert.AreEqual(healthResponse.HealthChecks.First().Results, subHealthData);
        }

        [Test]
        public async Task WhenAllHealthReportsAreNotHealthy_ReturnsUnhealthy()
        {
            var subHealthData = new Dictionary<string, object>()
            {
                { "some provider communication success", false }
            };
            var entryList = new Dictionary<string, HealthReportEntry>()
            {
                {"provider entries", new HealthReportEntry(HealthStatus.Unhealthy, "", new TimeSpan(), null, subHealthData) }
            };

            _mockHealthCheckService.HealthReport = new HealthReport(entryList, new TimeSpan());

            // Act
            var response = (ObjectResult)await _healthController.HealthCheckAsync();
            var healthResponse = (HealthCheckResponse)response.Value;

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, response.StatusCode);
            Assert.AreEqual(HealthStatus.Unhealthy.ToString(), healthResponse.Status);
            Assert.AreEqual(healthResponse.HealthChecks.First().Results, subHealthData);
        }

        private class MockHealthCheckService : HealthCheckService
        {
            public HealthReport HealthReport { get; set; }
            public override Task<HealthReport> CheckHealthAsync(Func<HealthCheckRegistration, bool> predicate, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(HealthReport);
            }
        }
    }
}