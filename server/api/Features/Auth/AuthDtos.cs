using System.ComponentModel.DataAnnotations;

namespace api.Features.Auth;

public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required] 
    public string Password { get; set; } = "";
}

public class LoginResponseDto
{
    public string Token { get; set; } = "";
    public string Role { get; set; } = "";
    public Guid UserId { get; set; }
    public string Email { get; set; } = "";
}