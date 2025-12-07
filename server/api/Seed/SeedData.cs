using System;
using System.Linq;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace api.Seed;

public static class SeedData
{
    public static void Initialize(MyDbContext context)
    {
        //Avoid duplicate seed data 
        if (context.Games.Any())
            return;

        var dk = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");

       //LAST WEEK
        var lastWeekSunday = new DateTime(2025, 11, 30, 9, 0, 0, DateTimeKind.Utc);

        var lastWeekCutoff = TimeOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeToUtc(
                new DateTime(2025, 11, 29, 17, 0, 0),
                dk
            )
        );

        var lastWeekGame = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = lastWeekSunday,
            Createdat = DateTime.UtcNow,
            Cutofftime = lastWeekCutoff,
            Winningnumbers = new() { 3, 7, 12 }   // ✔ juego con winners
        };

        context.Games.Add(lastWeekGame);


       //WEEK IDENTITY: SUNDAY OF THIS WEEK
        var thisWeekSunday = new DateTime(2025, 12, 07, 9, 0, 0, DateTimeKind.Utc);

        // cutoff Saturday December 6 17:00 DK
        var thisWeekCutoff = TimeOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeToUtc(
                new DateTime(2025, 12, 06, 17, 0, 0),
                dk
            )
        );

        var activeGame = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = thisWeekSunday,
            Createdat = DateTime.UtcNow,
            Cutofftime = thisWeekCutoff,
            Winningnumbers = null    // Current game  
        };

        context.Games.Add(activeGame);
        
        context.SaveChanges();
    }
}
