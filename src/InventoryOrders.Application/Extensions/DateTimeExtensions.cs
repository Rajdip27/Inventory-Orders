using Microsoft.AspNetCore.Http;
using NodaTime;

public static class DateTimeExtensions
{
    private static IHttpContextAccessor _httpContextAccessor;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static DateTimeOffset ToUserTime(this DateTimeOffset utc)
    {
        var context = _httpContextAccessor?.HttpContext;
        if (context == null) return utc;

        var ianaTimeZone = context.Session.GetString("UserTimeZone") ?? "UTC";

        try
        {
            // Get IANA timezone using NodaTime
            var tzdb = DateTimeZoneProviders.Tzdb;
            var zone = tzdb[ianaTimeZone];

            // Convert UTC to user timezone
            var instant = Instant.FromDateTimeOffset(utc);
            var zonedDateTime = instant.InZone(zone);

            return zonedDateTime.ToDateTimeOffset();
        }
        catch
        {
            return utc;
        }
    }
    /// <summary>
    /// Converts a DateTimeOffset to UTC
    /// </summary>
    public static DateTimeOffset ToUtc(this DateTimeOffset dateTime)
    {
        return dateTime.ToUniversalTime();
    }
}
