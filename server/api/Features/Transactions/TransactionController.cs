using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using dataaccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Transactions;

[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [Authorize(Roles="Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAll()
    {
        var transactions = await _transactionService.GetAllAync();
        return Ok(transactions);
    }
    
    //Player gets their own!!! balance
    [Authorize(Roles="Player")]
    [HttpGet("me/balance")]
    public async Task<ActionResult<decimal>> GetMyBalance()
    {
        var playerId = GetUserIdOrThrow();
        var balance = await _transactionService.GetBalanceAsync(playerId);
        return Ok(balance);
    }

    [Authorize(Roles="Admin")]
    [HttpGet("player/{playerId:guid}/balance")]
    public async Task<ActionResult<decimal>> GetBalanceForPlayer([FromRoute] Guid playerId)
    {
        var balance = await _transactionService.GetBalanceAsync(playerId);
        return Ok(balance);
    }

    [Authorize(Roles="Player")]
    [HttpPost]
    public async Task<ActionResult<Transaction>> Create([FromBody] CreateTransactionDto dto)
    {
        try
        {
            var playerId = GetUserIdOrThrow();
            var t = await _transactionService.CreatePendingAsync(
                playerId,
                dto.Amount,
                dto.MobilePayTransactionNumber
            );
            return Ok(t);
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
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(sub))
            throw new UnauthorizedAccessException("Missing sub claim");

        return Guid.Parse(sub);
    }
}