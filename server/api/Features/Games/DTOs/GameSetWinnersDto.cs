using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Features.Games.Dtos;

public class GameSetWinnersDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(3)]
    public List<int> WinningNumbers { get; set; } = new();
}