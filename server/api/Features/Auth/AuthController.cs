using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace api.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IPasswordService _passwords;
    private readonly AppOptions _opts;

    public AuthController(MyDbContext db, IPasswordService passwords, IOptions<AppOptions> opts)
    {
        _db = db;
        _passwords = passwords;
        _opts = opts.Value;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var admin = await _db.Admins.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email && !a.Isdeleted);

        if (admin != null)
        {
            if (!_passwords.Verify(admin.Password, dto.Password))
                return Unauthorized("Invalid credentials");

            var token = CreateJwt(admin.Adminid, admin.Email, "Admin");
            return Ok(new LoginResponseDto
            {
                Token = token,
                Role = "Admin",
                UserId = admin.Adminid,
                Email = admin.Email
            });
        }

        // Try playerr
        var player = await _db.Players.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email && !p.Isdeleted);

        if (player == null)
            return Unauthorized("Invalid credentials");

        if (!_passwords.Verify(player.Password, dto.Password))
            return Unauthorized("Invalid credentials");

        var playerToken = CreateJwt(player.Playerid, player.Email, "Player");
        return Ok(new LoginResponseDto
        {
            Token = playerToken,
            Role = "Player",
            UserId = player.Playerid,
            Email = player.Email
        });
    }

    private string CreateJwt(Guid userId, string email, string role)
    {

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, role)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.JwtSecret));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opts.JwtIssuer,
            audience: _opts.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}

}