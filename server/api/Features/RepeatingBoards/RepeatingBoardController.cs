using api.Features.RepeatingBoards.Dtos;
using dataaccess.Entities;
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

    [HttpPost("toggle")]
    public async Task<ActionResult<Board>> ToggleRepeatingBoard([FromBody] ToggleRepeatingBoardRequest request)
    {
        try
        {
            var board = await _repeatingBoardService.ToggleRepeatingBoard(request.BoardId, request.IsRepeating);
            return Ok(board);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
