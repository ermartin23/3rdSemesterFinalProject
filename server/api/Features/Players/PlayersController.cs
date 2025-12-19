using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Features.Players.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Players;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/players")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<PlayerResponseDto>>> GetAll()
    {
        var players = await _playerService.GetAllAsync();
        return Ok(players);
    }
 
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlayerResponseDto>> GetById(Guid id)
    {
        var player = await _playerService.GetByIdAsync(id);
        if (player == null) return NotFound();
        return Ok(player);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerResponseDto>> Create([FromBody] PlayerCreateRequestDto dto)
    {
        var created = await _playerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.PlayerId }, created);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PlayerResponseDto>> Update(Guid id, [FromBody] PlayerUpdateRequestDto dto)
    {
        var updated = await _playerService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated); 
    }
    
    [HttpPatch("{id:guid}/toggle-active")]
    public async Task<ActionResult<PlayerResponseDto>> ToggleActive(Guid id)
    {
        var updated = await _playerService.ToggleActiveAsync(id);
        return Ok(updated);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _playerService.SoftDeleteAsync(id);
        return NoContent();
    }
}