using System;
using System.ComponentModel.DataAnnotations;

namespace api.Features.Players.Dtos;

public class PlayerCreateRequestDto
{
    [Required] [MaxLength(200)] public string Name { get; set; } = null!;

    [Required] [MaxLength(50)] public string Phone { get; set; } = null!;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = null!;
    
    //we never return passwords on responses
    [Required, MinLength(6), MaxLength(200)]
    public string Password { get; set; } = null!;
    
    public bool Active { get; set; }
    
}

public class PlayerUpdateRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Phone { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = null!;
    
    [MinLength(6), MaxLength(200)]
    public string? Password { get; set; }
    
    public bool Active { get; set; }
}

public class PlayerResponseDto
{
    public Guid PlayerId { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}