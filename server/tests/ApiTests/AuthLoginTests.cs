using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.Features.Auth;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace tests.ApiTests;

public class AuthLoginTests
{
    private readonly MyDbContext _db;
    private readonly IPasswordService _passwords;
    private readonly AuthController _controller;

    public AuthLoginTests(MyDbContext db, IPasswordService passwords, AuthController controller)
    {
        _db = db;
        _passwords = passwords;
        _controller = controller;
    }

    [Fact]
    public async Task Login_Admin_Success_ReturnsToken_AndRoleAdmin()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();

        var adminId = Guid.NewGuid();
        var email = "admin@example.com";
        var password = "MyAdminPassword123!";

        _db.Admins.Add(new Admin
        {
            Adminid = adminId,
            Name = "Test Admin",
            Phone = "123",
            Email = email,
            Password = _passwords.Hash(password),
            Createdat = DateTime.UtcNow,
            Updatedat = DateTime.UtcNow,
            Isdeleted = false
        });

        await _db.SaveChangesAsync();

        var result = await _controller.Login(new LoginRequestDto
        {
            Email = email,
            Password = password
        });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<LoginResponseDto>(ok.Value);
        
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
        Assert.Equal("Admin", body.Role);
        Assert.Equal(adminId, body.UserId);
        Assert.Equal(email, body.Email);

        AssertJwt(body.Token, expectedSub: adminId, expectedRole: "Admin");
    }

    [Fact]
    public async Task Login_Player_Success_ReturnsToken_AndRolePlayer()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();

        var playerId = Guid.NewGuid();
        var email = "player@example.com";
        var password = "MyPlayerPassword123!";

        _db.Players.Add(new Player
        {
            Playerid = playerId,
            Name = "Test Player",
            Phone = "456",
            Email = email,
            Password = _passwords.Hash(password),
            Active = true,
            Createdat = DateTime.UtcNow,
            Updatedat = DateTime.UtcNow,
            Isdeleted = false
        });

        await _db.SaveChangesAsync();

        var result = await _controller.Login(new LoginRequestDto
        {
            Email = email,
            Password = password
        });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<LoginResponseDto>(ok.Value);
        
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
        Assert.Equal("Player", body.Role);
        Assert.Equal(playerId, body.UserId);
        Assert.Equal(email, body.Email);

        AssertJwt(body.Token, expectedSub: playerId, expectedRole: "Player");
    }

    private static void AssertJwt(string token, Guid expectedSub, string expectedRole)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        
        // issuer/audience checks
        Assert.Equal("DeadPigeonsAPI", jwt.Issuer);
        Assert.Contains("DeadPigeonsClient", jwt.Audiences);
        
        // sub claim
        var sub = jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        Assert.Equal(expectedSub.ToString(), sub);
        
        // role claim
        var role = jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        Assert.Equal(expectedRole, role);
    }

}