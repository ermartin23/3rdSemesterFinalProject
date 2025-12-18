using System;
using System.Threading.Tasks;
using api.Features.Games;
using api.Features.Games.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace tests.ApiTests;

public class GameServiceCutoffTests
{
    private MyDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MyDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCorrectCutoff_WhenSundayGiven()
    {
        // Sunday Jan 5 2025 (UTC)
        var sundayUtc = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc);

        var dto = new GameCreateRequestDto
        {
            Weekidentity = sundayUtc
        };

        var db = CreateDb();
        var clock = new FakeClock { UtcNow = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var service = new GameService(db, clock);


        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        // CutoffTime is stored as UTC inside TimeOnly
        // Expected cutoff: Saturday Jan 4 2025 at 17:00 DK time = 16:00 UTC
        var expectedCutoffUtc = new DateTime(2025, 1, 4, 16, 0, 0, DateTimeKind.Utc);
        var expectedCutoff = TimeOnly.FromDateTime(expectedCutoffUtc);

        Assert.Equal(expectedCutoff, result.Cutofftime);
    }
}