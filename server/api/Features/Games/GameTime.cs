using TimeZoneConverter;

namespace api.Features.Games;

public static class GameTime
{
    public static DateTime GetCutoffUtcFromWeekSundayUtc(DateTime weekSundayUtc)
    {
        var dk = TZConvert.GetTimeZoneInfo("Europe/Copenhagen");
        var weekDk = TimeZoneInfo.ConvertTimeFromUtc(weekSundayUtc, dk);
        var cutoffDk = weekDk.AddDays(-1).Date.AddHours(17); // Sat 17:00 DK
        return TimeZoneInfo.ConvertTimeToUtc(cutoffDk, dk);
    }
}