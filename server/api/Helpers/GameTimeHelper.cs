using System;

namespace api.Helpers;

public static class GameTimeHelper
{
    private static readonly TimeZoneInfo DkZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");

    // Calculates cutoff as Saturday 17:00 DK time, converted to UTC
    public static TimeOnly GetCutoffTimeUtc(DateTime weekSundayUtc)
    {
        // Convert the input Sunday from UTC → DK local time
        var weekDk = TimeZoneInfo.ConvertTimeFromUtc(weekSundayUtc, DkZone);

        // Cutoff is Saturday 17:00 of the same week
        var cutoffDk = weekDk.AddDays(-1).Date.AddHours(17);

        // Convert back to UTC
        var cutoffUtc = TimeZoneInfo.ConvertTimeToUtc(cutoffDk, DkZone);

        return TimeOnly.FromDateTime(cutoffUtc);
    }

    // Returns next Sunday at 00:00 UTC
    public static DateTime GetNextSundayUtc(DateTime previousSundayUtc)
    {
        return DateTime.SpecifyKind(previousSundayUtc.AddDays(7), DateTimeKind.Utc);
    }
}