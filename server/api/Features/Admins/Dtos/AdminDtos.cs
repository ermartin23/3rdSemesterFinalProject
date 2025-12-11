using System.ComponentModel.DataAnnotations;

namespace api.Features.Admins.Dtos;


public class AdminCreateRequestDto
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

    // For now this is plain text; later you can hash it when storing.
    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string Password { get; set; } = null!;
}

public class AdminUpdateRequestDto
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
    //  I can add a separate ChangePassword DTO later.
}

public class AdminResponseDto
{
    public Guid AdminId { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}