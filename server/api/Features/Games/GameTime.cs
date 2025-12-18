namespace api.Features.Games;

public class GameTime
{
    public static DateTime GetCutoffUtcFromWeekSundayUtc(DateTime weekSundayUtc)
    {
        var dk = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");
        var weekDk = TimeZoneInfo.ConvertTimeFromUtc(weekSundayUtc, dk);
        var cutoffDk = weekDk.AddDays(-1).Date.AddHours(17); 
        return TimeZoneInfo.ConvertTimeToUtc(cutoffDk, dk);
    }
}