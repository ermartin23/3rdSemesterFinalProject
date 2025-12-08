using System.Collections.Generic;
using System.Linq;
using api.Features.Games.Dtos;
using dataaccess.Entities;

namespace api.Features.Games.Mappings;

public static class GameMappings
{
    public static GameResponseDto ToGameResponseDto(this Game g)
    {
        return new GameResponseDto
        {
            Gameid = g.Gameid,
            Weekidentity = g.Weekidentity,
            Createdat = g.Createdat,
            Cutofftime = g.Cutofftime,
            Winningnumbers = g.Winningnumbers,
            IsOpen = g.Winningnumbers == null || g.Winningnumbers.Count == 0
        };
    }

    public static List<GameResponseDto> ToGameResponseDtos(this List<Game> games)
    {
        return games.Select(g => g.ToGameResponseDto()).ToList();
    }
}