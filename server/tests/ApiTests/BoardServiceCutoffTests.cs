using System;
using System.Threading.Tasks;
using api;
using api.Features.Boards;
using api.Features.Boards.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace tests.ApiTests;

public class BoardServiceCutoffTests
{
    private MyDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MyDbContext(options);
    }
    /*
    [Fact]
    public async Task CreateBoard_ShouldThrow_WhenAfterCutoffTime()
    {
        // Arrange
        var db = CreateDbContext();

        var sundayUtc = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        var cutoffUtc = new DateTime(2025, 1, 4, 16, 0, 0, DateTimeKind.Utc);

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = sundayUtc,
            Createdat = DateTime.UtcNow,
            Cutofftime = TimeOnly.FromDateTime(cutoffUtc),
            Winningnumbers = null,
            Isdeleted = false
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var playerId = Guid.NewGuid();

        var request = new CreateBoardRequest
        {
            GameId = game.Gameid,
            ChosenNumbers = new() { 1, 5, 9, 10, 11}
        };

        var transactionService = new FakeTransactionService();
        var boardService = new BoardService(db, transactionService);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await boardService.CreateBoardAsync(playerId, request);
        });
    }
    
    private class FakeTransactionService : ITransactionService
    {
        public Task<IEnumerable<Transaction>> GetAllAync()
            => Task.FromResult(Enumerable.Empty<Transaction>());

        public Task<Transaction> CreatePendingAsync(Guid playerId, int amount, string mobilePayTransactionNumber)
            => throw new NotImplementedException();

        public Task ApproveAsync(Guid transactionId)
            => throw new NotImplementedException();

        public Task RejectAsync(Guid transactionId)
            => throw new NotImplementedException();

        public Task<decimal> GetBalanceAsync(Guid playerId)
            => Task.FromResult(1000m); // plenty of balance
    }
    */
    
    
}