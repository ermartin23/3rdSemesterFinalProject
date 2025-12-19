using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.Features.Boards.Dtos;
using api.Features.RepeatingBoards;
using dataaccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Boards;

[Authorize]
[Route("api/boards")]
[ApiController]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;
    private readonly IRepeatingBoardService _repeatingBoardService;

    public BoardController(IBoardService boardService, IRepeatingBoardService repeatingBoardService)
    {
        _boardService = boardService;
        _repeatingBoardService = repeatingBoardService;
    }

    [Authorize(Roles="Admin")]
    [HttpGet]
    public async Task<ActionResult<List<Board>>> GetAllBoards()
    {
        var boards = await _boardService.GetAllBoards();
        return Ok(boards);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Board>> GetBoardById(Guid id)
    {
        Board? board;

        if (User.IsInRole("Admin"))
        {
            board = await _boardService.GetBoardById(id);
        }
        else if (User.IsInRole("Player"))
        {
            var playerId = GetUserIdOrThrow();
            board = await _boardService.GetBoardByIdForPlayer(id, playerId);
        }
        else
        {
            return Forbid();
        }

        if (board == null) return NotFound("Board not found");
        return Ok(board);
    }

    [Authorize(Roles="Player")]
    [HttpPost]
    public async Task<ActionResult<BoardResponse>> CreateBoard([FromBody] CreateBoardRequest request)
    {
        try
        {
            var playerId = GetUserIdOrThrow();
            var board = await _boardService.CreateBoardAsync(playerId, request);
            
            if (request.Repeat)
            {
                await _repeatingBoardService.ToggleRepeatingBoard(playerId, board.Boardid, true);
            }
            
            var response = new BoardResponse(
                board.Boardid,
                board.Gameid,
                board.Playerid,
                board.Chosennumbers,
                board.Price
            );
            return Ok(response);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [Authorize(Roles="Admin")]
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
    
    [Authorize(Roles="Player")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBoard(Guid id)
    {
        var playerId = GetUserIdOrThrow();

        var success = await _boardService.DeleteBoard(id, playerId);
        if (!success) return NotFound("Board not found");
        return NoContent();
    }
    
    private Guid GetUserIdOrThrow()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(sub))
            throw new UnauthorizedAccessException("Missing sub claim");

        return Guid.Parse(sub);
    }
    
    [Authorize(Roles = "Player")]
    [HttpGet("me")]
    public async Task<ActionResult<List<BoardHistoryResponse>>> GetMyBoards()
    {
        var playerId = GetUserIdOrThrow();
        var boards = await _boardService.GetBoardsForPlayerAsync(playerId);

        var response = boards.Select(b =>
        {
            var weekDate = b.Game.Weekidentity; 
            var week = ISOWeek.GetWeekOfYear(weekDate);
            var year = ISOWeek.GetYear(weekDate);

            return new BoardHistoryResponse(
                b.Boardid,
                b.Gameid,
                weekDate,
                week,
                year,
                b.Chosennumbers,
                b.Price,
                (b.Repeatingboard != null && b.Repeatingboard.Isrepeating)
                ? b.Repeatingboardid
                : null
            );
        }).ToList();

        return Ok(response);
    }
}