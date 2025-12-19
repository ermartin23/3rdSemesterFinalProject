using System.Collections.Generic;
using System.Linq;
using api.Features.Players.Dtos;
using dataaccess.Entities;

namespace api.Features.Players;

public static class PlayerMapping
{
    public static PlayerResponseDto ToPlayerResponseDto(this Player entity)
    {
        return new PlayerResponseDto
        {
            PlayerId = entity.Playerid,
            Name = entity.Name,
            Phone = entity.Phone,
            Email = entity.Email,
            Active = entity.Active,
            CreatedAt = entity.Createdat,
            UpdatedAt = entity.Updatedat
        };
    }

    public static List<PlayerResponseDto> ToPlayerResponseDtos(this IEnumerable<Player> entities)
    {
        return entities.Select(e => e.ToPlayerResponseDto()).ToList(); 
    }
}