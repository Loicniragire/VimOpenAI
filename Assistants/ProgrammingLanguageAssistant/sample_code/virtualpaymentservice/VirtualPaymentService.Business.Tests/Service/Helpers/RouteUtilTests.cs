using NUnit.Framework;
using System;
using VirtualPaymentService.Business.Service.Helpers;

namespace VirtualPaymentService.Business.Tests.Service.Helpers
{
    [TestFixture]
    public class RouteUtilTests
    {
        #region FormatRoute() Test
        [Test]
        public void WhenThereAreNoRouteInterpolationLocations_Then_FormatRoute_ReturnsProperRoute()
        {
            // Arrange
            string route = "myroute/thisIsAtest";
            string entityId = Guid.NewGuid().ToString();

            // Act
            string url = RouteUtil.FormatRoute(route, entityId);

            // Assert
            Assert.AreEqual($"{route}/{entityId}", url);
        }

        [Test]
        public void WhenTheSameNumberOfInterpolationLocations_AndEntityIdsAreProvided_Then_FormatRoute_ReturnsProperRoute()
        {
            // Arrange
            string route = "/{entityId1}/{entityId2}/thisIsAtest/";
            string entityId1 = Guid.NewGuid().ToString();
            string entityId2 = Guid.NewGuid().ToString();

            // Act
            string url = RouteUtil.FormatRoute(route, entityId1, entityId2);

            // Assert
            Assert.AreEqual($"{entityId1}/{entityId2}/thisIsAtest", url);
        }

        [Test]
        public void WhenThereIsOneMoreEntityId_ThanInterpolationLocations_Then_FormatRoute_ReturnsProperRoute()
        {
            // Arrange
            string route = "/{entityId1}/{entityId2}/thisIsAtest/";
            string entityId1 = Guid.NewGuid().ToString();
            string entityId2 = Guid.NewGuid().ToString();
            string entityId3 = Guid.NewGuid().ToString();

            // Act
            string url = RouteUtil.FormatRoute(route, entityId1, entityId2, entityId3);

            // Assert
            Assert.AreEqual($"{entityId1}/{entityId2}/thisIsAtest/{entityId3}", url);
        }

        [Test]
        public void WhenThereAreNoEntityIdsProvided_Then_FormatRoute_ReturnsProperRoute()
        {
            // Arrange & Act
            string url = RouteUtil.FormatRoute("/myDearEndpoint/thisIsAtest/");

            // Assert
            Assert.AreEqual("myDearEndpoint/thisIsAtest", url);
        }

        [TestCase("/{varOne}/thisIsAtest/", "1235432", "myLeaseIdBaby", "third", Description = "When there are too many entityIds")]
        [TestCase("/{varOne}/{thisIsAtest}/", "1235432", Description = "When there are too few entityIds")]
        public void WhenThereIsAMismatch_Then_FormatRoute_ThrowsArgumentOutOfRangeException(string route, params string[] entityIds)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => RouteUtil.FormatRoute(route, entityIds));
        }
        #endregion
    }
}
