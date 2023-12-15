using System;
using System.Reflection;

namespace VirtualPaymentService.Business.Tests.Service.Helpers;

public sealed class FakeLocalTimeZone : IDisposable 
{
    private readonly TimeZoneInfo _actualLocalTimeZoneInfo;

    private static void SetLocalTimeZone(TimeZoneInfo timeZoneInfo)
    {
        var info = typeof(TimeZoneInfo).GetField("s_cachedData", BindingFlags.NonPublic | BindingFlags.Static);
        var cachedData = info?.GetValue(null);

        var field = cachedData?.GetType().GetField("_localTimeZone",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Instance);
        field?.SetValue(cachedData, timeZoneInfo);
    }

    public FakeLocalTimeZone(TimeZoneInfo timeZoneInfo)
    {
        _actualLocalTimeZoneInfo = TimeZoneInfo.Local;
        SetLocalTimeZone(timeZoneInfo);
    }

    public void Dispose()
    {
        SetLocalTimeZone(_actualLocalTimeZoneInfo);
    }
}