using System.Security.Claims;
using api.Features.RepeatingBoards.Dtos;
using dataaccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.RepeatingBoards;

[Route("api/[controller]")]
[ApiController]
public class RepeatingBoardController : ControllerBase
{
    private readonly IRepeatingBoardService _repeatingBoardService;

    public RepeatingBoardController(IRepeatingBoardService repeatingBoardService)
    {
        _repeatingBoardService = repeatingBoardService;
    }

    [Authorize(Roles="Player")]
    [HttpPost("toggle")]
    public async Task<IActionResult> ToggleRepeatingBoard([FromBody] ToggleRepeatingBoardRequest request)
    {
        var playerId = GetUserIdOrThrow();

        try
        {
            await _repeatingBoardService.ToggleRepeatingBoard(playerId, request.BoardId, request.IsRepeating);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    private Guid GetUserIdOrThrow()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(sub))
            throw new UnauthorizedAccessException("Missing sub claim");

        return Guid.Parse(sub);
    }

}
