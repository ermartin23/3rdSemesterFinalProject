using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Features.Games.Dtos;

namespace api.Features.Games;

public interface IGameService
{
    Task<List<GameResponseDto>> GetAllAsync();
    Task<GameResponseDto?> GetByIdAsync(Guid id);
    Task<GameResponseDto> CreateAsync(GameCreateRequestDto dto);
    Task<GameResponseDto> SetWinningNumbersAsync(Guid id, GameSetWinnersDto dto);
    Task<GameDetailsResponseDto?> GetDetailsAsync(Guid id);
    Task<WinnerDto?> GetLatestWinningNumbersAsync();
}