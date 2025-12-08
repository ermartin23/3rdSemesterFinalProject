using dataaccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
[Route("transaction")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAll()
    {
        var transactions = await _transactionService.GetAllAync();
        return Ok(transactions);
    }

    [HttpGet("player/{playerId:guid}/balance")]
    public async Task<ActionResult<decimal>> GetBalance([FromRoute] Guid playerId)
    {
        var balance = await _transactionService.GetBalanceAsync(playerId);
        return Ok(balance);
    }

    [HttpPost("player/{playerId:guid}/transaction")]
    public async Task<ActionResult<Transaction>> Create(
        [FromRoute] Guid playerId,
        [FromBody] CreateTransactionDto createTransactionDto)
    {
        try
        {
            var t = await _transactionService.CreatePendingAsync(
                playerId,
                createTransactionDto.Amount,
                createTransactionDto.MobilePayTransactionNumber
            );
            return Ok(t);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

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
}