using api;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace tests.ApiTests;

public class TransactionServiceTests
{
    private readonly MyDbContext _dbContext;
    private readonly ITransactionService _transactionService;

    public TransactionServiceTests(MyDbContext dbContext, ITransactionService transactionService)
    {
        _dbContext = dbContext;
        _transactionService = transactionService;
    }

    private async Task<Player> SeedPlayerAsync()
    {
        var player = new Player
        {
            Playerid = Guid.NewGuid(),
            Name = "Bob Smith",
            Phone = "49589490",
            Email = "bobs@example.com",
            Password = "password",
            Active = false,
            Createdat = DateTime.UtcNow,
            Updatedat = DateTime.UtcNow,
            Isdeleted = false
        };
        _dbContext.Players.Add(player);
        await _dbContext.SaveChangesAsync();
        return player;
    }

    private async Task<Game> SeedGameAsync()
    {
        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = DateTime.UtcNow,
            Cutofftime = new TimeOnly(12, 0),
            Createdat = DateTime.UtcNow,
            Isdeleted = false,
            Deletedat = null
        };
        
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();
        return game;
    }

    private async Task<Board> SeedBoardAsync(Guid playerId, decimal price, bool isDeleted = false)
    {
        var game = await SeedGameAsync();
        
        var board = new Board
        {
            Boardid = Guid.NewGuid(),
            Playerid = playerId,
            Gameid = game.Gameid,
            
            Chosennumbers = [1, 2, 3, 4, 5],
            Iswinningboard = false,
            Price = price,
            
            Isdeleted = isDeleted,
            Deletedat = null,
            Repeatingboardid = null
        };
        
        _dbContext.Boards.Add(board);
        await _dbContext.SaveChangesAsync();
        return board;
    }

    //GetAllAsync
    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmpty()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var result = (await _transactionService.GetAllAsync()).ToList();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAll_InDescendingCreatedAt()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();

        var older = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Amount = 100,
            Mobilepaytransactionnumber = "94038485968",
            Status = "pending",
            Createdat = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Isdeleted = false
        };

        var newer = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Amount = 200,
            Mobilepaytransactionnumber = "27364859273",
            Status = "pending",
            Createdat = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
            Isdeleted = false
        };
        
        _dbContext.Transactions.AddRange(older, newer);
        await _dbContext.SaveChangesAsync();
        
        _dbContext.ChangeTracker.Clear();
        
        var result = (await _transactionService.GetAllAsync()).ToList();
        
        Assert.Equal(2, result.Count);
        Assert.Equal(newer.Transactionid, result[0].Transactionid);
        Assert.Equal(older.Transactionid, result[1].Transactionid);
        
        Assert.NotNull(result[0].Player);
    }
    
    //CreatePendingAsync
    [Fact]
    public async Task CreatePendingAsync_CreatesPendingTransaction()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();

        var created = await _transactionService.CreatePendingAsync(player.Playerid, 100, "73648394029");
        
        var inDB = await _dbContext.Transactions.FirstAsync(t => t.Transactionid == created.Transactionid);
        
        Assert.Equal(player.Playerid, inDB.Playerid);
        Assert.Equal(100, inDB.Amount);
        Assert.Equal("73648394029", inDB.Mobilepaytransactionnumber);
        Assert.Equal("pending", inDB.Status);
        Assert.False(inDB.Isdeleted);
        Assert.Null(inDB.Deletedat);
        Assert.NotEqual(default, inDB.Createdat);
    }

    [Fact]
    public async Task CreatePendingAsync_AmountZero_Throws()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();
        
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _transactionService.CreatePendingAsync(player.Playerid, 0, "27485937584"));
    }

    [Fact]
    public async Task CreatePendingAsync_EmptyMobilePayTransactionNumber_Throws()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();
        
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _transactionService.CreatePendingAsync(player.Playerid, 100, " "));
    }
    
    //GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsTransaction()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var  player = await SeedPlayerAsync();

        var transaction = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Amount = 100,
            Mobilepaytransactionnumber = "274856356475",
            Status = "pending",
            Createdat = DateTime.UtcNow,
            Isdeleted = false
        };
        
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        
        var found = await _transactionService.GetByIdAsync(transaction.Transactionid);
        
        Assert.NotNull(found);
        Assert.Equal(transaction.Transactionid, found!.Transactionid);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNull()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var found = await  _transactionService.GetByIdAsync(Guid.NewGuid());
        
        Assert.Null(found);
    }
    
    //GetBalanceAsync
    [Fact]
    public async Task GetBalanceAsync_ReturnsApprovedSumMinusBoardCost()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();

        _dbContext.Transactions.AddRange(
            new Transaction
            {
                Transactionid = Guid.NewGuid(),
                Playerid = player.Playerid,
                Amount = 100,
                Mobilepaytransactionnumber = "47593849583",
                Status = "approved",
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            },
            new Transaction
            {
                Transactionid = Guid.NewGuid(),
                Playerid = player.Playerid,
                Amount = 25,
                Mobilepaytransactionnumber = "12837492048",
                Status = "pending",
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            }
        );
        
        await _dbContext.SaveChangesAsync();
        
        await SeedBoardAsync(player.Playerid, price: 30m, isDeleted: false);
        await SeedBoardAsync(player.Playerid, price: 999m, isDeleted: true);
        
        var balance = await _transactionService.GetBalanceAsync(player.Playerid);
        
        Assert.Equal(70m, balance);
    }
    
    [Fact]
    public async Task GetBalanceAsync_WhenMissing_ReturnsZero()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();
        
        var balance = await  _transactionService.GetBalanceAsync(player.Playerid);
        
        Assert.Equal(0, balance);
    }
    
    //ApproveAsync
    [Fact]
    public async Task ApproveAsync_Pending_BecomesApproved()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();

        var transaction = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Amount = 100,
            Mobilepaytransactionnumber = "4689378493",
            Status = "pending",
            Createdat = DateTime.UtcNow,
            Isdeleted = false
        };
        
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        
        await  _transactionService.ApproveAsync(transaction.Transactionid);
        
        var inDB = await _dbContext.Transactions.FirstAsync(x => x.Transactionid == transaction.Transactionid);
        Assert.Equal("approved", inDB.Status);
    }
    
    [Fact]
    public async Task ApproveAsync_AlreadyApproved_DoesNothing()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();

        var transaction = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Amount = 100,
            Mobilepaytransactionnumber = "84684958385",
            Status = "approved",
            Createdat = DateTime.UtcNow,
            Isdeleted = false
        };
        
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        
        await  _transactionService.ApproveAsync(transaction.Transactionid);
        
        var inDB = await _dbContext.Transactions.FirstAsync(x => x.Transactionid == transaction.Transactionid);
        Assert.Equal("approved", inDB.Status);
    }

    [Fact]
    public async Task ApproveAsync_WhenMissing_ThrowsKeyNotFound()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _transactionService.ApproveAsync(Guid.NewGuid()));
    }
    
    [Fact]
    public async Task ApproveAsync_WhenDeclined_ThrowsInvalidOperation()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();

        var transaction = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Amount = 100,
            Mobilepaytransactionnumber = "76849385748",
            Status = "declined",
            Createdat = DateTime.UtcNow,
            Isdeleted = false
        };
        
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _transactionService.ApproveAsync(transaction.Transactionid));
    }

    [Fact]
    public async Task ApproveAsync_WhenWouldCauseNegativeBalance_Throws()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();

        await SeedBoardAsync(player.Playerid, price: 100, isDeleted: false);

        var transaction = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Amount = 50,
            Mobilepaytransactionnumber = "39485738574",
            Status = "pending",
            Createdat = DateTime.UtcNow,
            Isdeleted = false
        };
        
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _transactionService.ApproveAsync(transaction.Transactionid));
    }

    //RejectAsync
    [Fact]
    public async Task RejectAsync_Pending_BecomesDeclined()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();

        var transaction = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Amount = 200,
            Mobilepaytransactionnumber = "62875927382",
            Status = "pending",
            Createdat = DateTime.UtcNow,
            Isdeleted = false
        };
        
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        
        await _transactionService.RejectAsync(transaction.Transactionid);
        
        var inDB = await _dbContext.Transactions.FirstAsync(x => x.Transactionid == transaction.Transactionid);
        Assert.Equal("declined", inDB.Status);
    }

    [Fact]
    public async Task RejectAsync_WhenMissing_ThrowsKeyNotFound()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _transactionService.RejectAsync(Guid.NewGuid()));
    }
    
    [Fact]
    public async Task RejectAsync_WhenApproved_ThrowsInvalidOperation()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        
        var player = await SeedPlayerAsync();

        var transaction = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Amount = 200,
            Mobilepaytransactionnumber = "25364785930",
            Status = "approved",
            Createdat = DateTime.UtcNow,
            Isdeleted = false
        };
        
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _transactionService.RejectAsync(transaction.Transactionid));
    }
}