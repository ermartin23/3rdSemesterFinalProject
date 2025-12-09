using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Features.Games.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Games;

[ApiController]
[Route("api/games")]
public class GameController : ControllerBase
{
    private readonly IGameService _svc;

    public GameController(IGameService svc)
    {
        _svc = svc;
    }

    // GET api/games
    [HttpGet]
    public async Task<ActionResult<List<GameResponseDto>>> GetAll()
    {
        var result = await _svc.GetAllAsync();
        return Ok(result);
    }

    // GET api/games/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GameResponseDto>> GetById(Guid id)
    {
        var game = await _svc.GetByIdAsync(id);
        if (game == null)
            return NotFound();

        return Ok(game);
    }

    // POST api/games
    [HttpPost]
    public async Task<ActionResult<GameResponseDto>> Create([FromBody] GameCreateRequestDto dto)
    {
        try
        {
            var created = await _svc.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), 
                new { id = created.Gameid }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST api/games/{id}/winners
    [HttpPost("{id:guid}/winners")]
    public async Task<ActionResult<GameResponseDto>> SetWinners(Guid id, [FromBody] GameSetWinnersDto dto)
    {
        try
        {
            var updated = await _svc.SetWinningNumbersAsync(id, dto);
            return Ok(updated);
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
    
    // Jeg er Emre
    [HttpGet("id:guid/details")]
    public async Task<ActionResult<GameDetailsResponseDto>> GetDetails(Guid id)
    {
        var details = await _svc.GetDetailsAsync(id);
        if (details == null)
            return NotFound();

        return Ok(details);
    }
}