using System.Collections.Generic;
using System.Linq;
using api.Features.Games.Dtos;
using dataaccess.Entities;

namespace api.Features.Games.Mappings;

public static class GameMappings
{
    public static GameResponseDto ToGameResponseDto(this Game g)
    {
        var cutoffUtc = GameTime.GetCutoffUtcFromWeekSundayUtc(g.Weekidentity);
        var isOpen = g.Winningnumbers == null || g.Winningnumbers.Count == 0;

        return new GameResponseDto
        {
            Gameid = g.Gameid,
            Weekidentity = g.Weekidentity,
            Createdat = g.Createdat,
            Cutofftime = g.Cutofftime,
            Winningnumbers = g.Winningnumbers,
            IsOpen = isOpen,

            CutoffUtc = cutoffUtc,
            CanSetWinnersNow = isOpen && DateTime.UtcNow >= cutoffUtc
        };
    }

    public static List<GameResponseDto> ToGameResponseDtos(this List<Game> games)
        => games.Select(g => g.ToGameResponseDto()).ToList();
}