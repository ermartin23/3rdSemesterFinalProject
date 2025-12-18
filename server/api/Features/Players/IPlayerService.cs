using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Features.Players.Dtos;

namespace api.Features.Players;

public interface IPlayerService
{
    Task<List<PlayerResponseDto>> GetAllAsync();
    Task<PlayerResponseDto?> GetByIdAsync(Guid id);
    Task<PlayerResponseDto> CreateAsync(PlayerCreateRequestDto dto);
    Task<PlayerResponseDto> UpdateAsync(Guid id, PlayerUpdateRequestDto dto);
    Task<PlayerResponseDto> ToggleActiveAsync(Guid id);
    Task SoftDeleteAsync(Guid id);
}