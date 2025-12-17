using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Transactions;

[ApiController]
[Route("api/[controller]")]

public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly MyDbContext _myDbContext;
    
    public TransactionController(ITransactionService transactionService, MyDbContext myDbContext)
    {
        _transactionService = transactionService;
        _myDbContext = myDbContext;
    }

    [Authorize(Roles="Admin")]
    [HttpGet("admin/transactions")]
    public async Task<ActionResult> GetAll()
    {
        var transactions = await _myDbContext.Transactions
            .AsNoTracking()
            .OrderByDescending(t =>  t.Createdat)
            .Select(t => new
            {
                transactionId = t.Transactionid,
                playerId = t.Playerid,
                playerEmail = t.Player.Email,
                playerName = t.Player.Name,
                amount = t.Amount,
                mobilePayTransactionNumber = t.Mobilepaytransactionnumber,
                status = t.Status,
                createdat = t.Createdat
            })
            .ToListAsync();
        
        return Ok(transactions);
    }
    
    //Player gets their own!!! balance
    [Authorize(Roles="Player")]
    [HttpGet("player/balance")]
    public async Task<ActionResult<decimal>> GetMyBalance()
    {
        var playerId = GetUserIdOrThrow();
        var balance = await _transactionService.GetBalanceAsync(playerId);
        return Ok(balance);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Transaction>> GetById([FromRoute] Guid id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        
        if (transaction is null) return NotFound();
        
        return Ok(transaction);
    }

    [Authorize(Roles = "Player")]
    [HttpGet("player/transactions")]
    public async Task<ActionResult> GetMyTransactions()
    {
        var playerId = GetUserIdOrThrow();
        
        var tx = await _myDbContext.Transactions
            .AsNoTracking()
            .Where(t => t.Playerid == playerId)
            .OrderByDescending(t => t.Createdat)
            .Select(t => new
            {
                transactionId = t.Transactionid,
                amount = t.Amount,
                mobilepaytransactionnumber = t.Mobilepaytransactionnumber,
                status = t.Status,
                createdat = t.Createdat
            })
            .ToListAsync();
        return Ok(tx);
    }
    
    [Authorize(Roles = "Player")]
    [HttpPost("player/createtransaction")]
    public async Task<ActionResult<Transaction>> Create([FromBody] CreateTransactionDto dto)
    {
        try
        {
            var playerId = GetUserIdOrThrow();
            var t = await _transactionService.CreatePendingAsync(playerId, dto.Amount, dto.MobilePayTransactionNumber);
            return Ok(t);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles="Admin")]
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve([FromRoute] Guid id)
    {
        try
        {
            await _transactionService.ApproveAsync(id);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles="Admin")]
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject([FromRoute] Guid id)
    {
        try
        {
            await _transactionService.RejectAsync(id);
            return Ok();
        }
        catch  (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    private Guid GetUserIdOrThrow()
    {
        var sub = 
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrWhiteSpace(sub))
            throw new UnauthorizedAccessException("Missing sub claim");

        return Guid.Parse(sub);
    }
}