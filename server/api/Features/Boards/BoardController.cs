using api.Features.Boards.Dtos;
using dataaccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Boards;

[Route("api/[controller]")]
[ApiController]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;

    public BoardController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Board>>> GetAllBoards()
    {
        var boards = await _boardService.GetAllBoards();
        return Ok(boards);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Board>> GetBoardById(Guid id)
    {
        var board = await _boardService.GetBoardById(id);
        if (board == null) return NotFound("Board not found");
        return Ok(board);
    }

    [HttpPost]
    public async Task<ActionResult<Board>> CreateBoard([FromBody] CreateBoardRequest request)
    {
        try
        {
            var board = await _boardService.CreateBoard(request);
            return Ok(board);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBoard(Guid id, [FromBody] UpdateBoardRequest request)
    {
        try
        {
            var board = await _boardService.UpdateBoard(id, request);
            if (board == null) return NotFound("Board not found");
            return Ok(board);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBoard(Guid id)
    {
        var success = await _boardService.DeleteBoard(id);
        if (!success) return NotFound("Board not found");
        return NoContent();
    }
}