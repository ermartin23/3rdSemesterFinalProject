using System;

namespace api.Helpers;

public static class GameTimeHelper
{
    private static readonly TimeZoneInfo DkZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");
    
    public static TimeOnly GetCutoffTimeUtc(DateTime weekSundayUtc)
    {
        var weekDk = TimeZoneInfo.ConvertTimeFromUtc(weekSundayUtc, DkZone);
        
        var cutoffDk = weekDk.AddDays(-1).Date.AddHours(17);
        
        var cutoffUtc = TimeZoneInfo.ConvertTimeToUtc(cutoffDk, DkZone);

        return TimeOnly.FromDateTime(cutoffUtc);
    }
    
    public static DateTime GetNextSundayUtc(DateTime previousSundayUtc)
    {
        return DateTime.SpecifyKind(previousSundayUtc.AddDays(7), DateTimeKind.Utc);
    }
}