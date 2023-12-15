using NUnit.Framework;
using System;
using VirtualPaymentService.Business.Service.Helpers;

namespace VirtualPaymentService.Business.Tests.Service.Helpers
{
    [TestFixture]
    public class DateUtilTests
    {
        [TestCase(0, 17, 0, 0)]
        [TestCase(1, 17, 0, 1)]
        [TestCase(2, 17, 0, 2)]
        [TestCase(3, 17, 0, 3)]
        [TestCase(-1, 17, 0, -1)]
        public void GetAuthTotalByAuthExpireDaysInPast_ValidateResponse(
            int expectedDaysInPast, int expectedHour, int expectedMin, int authExpireDays)
        {
            using (new FakeLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time")))
            {
                var now = DateTime.Now;
                var dstOffset = now.IsDaylightSavingTime() ? 1 : 0;
                var expectedHourWithOffset = expectedHour + dstOffset;
                expectedDaysInPast = now.Hour >= expectedHourWithOffset ? expectedDaysInPast - 1 : expectedDaysInPast;

                var expectedDate = now.AddDays(-expectedDaysInPast);

                var startDate = DateUtil.GetStartDateByAuthExpireDays(authExpireDays, now);

                Assert.AreEqual(expectedDate.Day, startDate.Day);
                Assert.AreEqual(expectedHourWithOffset, startDate.Hour);
                Assert.AreEqual(expectedMin, startDate.Minute);
            }
        }

        [TestCase(3, "05/15/2022 17:00:00", "05/12/2022 18:00:00")]
        [TestCase(3, "05/15/2022 18:00:00", "05/13/2022 18:00:00")]
        [TestCase(3, "12/31/2022 16:00:00", "12/28/2022 17:00:00")]
        [TestCase(3, "12/31/2022 17:00:00", "12/29/2022 17:00:00")]
        public void GetAuthTotalByAuthExpireDaysInPast_ValidateResponse_Verify_DST_And_UTC_NewYear_Switch(
            int authExpireDays, DateTime testNowDate, DateTime expectedStartDate)
        {
            using (new FakeLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time")))
            {
                var startDate = DateUtil.GetStartDateByAuthExpireDays(authExpireDays, testNowDate);

                Assert.AreEqual(expectedStartDate.Year, startDate.Year);
                Assert.AreEqual(expectedStartDate.Month, startDate.Month);
                Assert.AreEqual(expectedStartDate.Day, startDate.Day);
                Assert.AreEqual(expectedStartDate.Hour, startDate.Hour);
                Assert.AreEqual(expectedStartDate.Minute, startDate.Minute);
            }
        }
    }
}
