using api.Features.Admins;
using api.Features.Admins.Dtos;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Xunit;
using api.Features.Admins;


namespace tests.ApiTests;

public class AdminServiceTests
{
    private readonly MyDbContext _db;
    private readonly IAdminService _adminService;

    public AdminServiceTests(MyDbContext db, IAdminService adminService)
    {
        _db = db;
        _adminService = adminService;
    }

    [Fact]
    public async Task CreateAsync_Creates_Admin_NotDeleted()
    {
        //clean DB for this test
        await _db.Database.EnsureDeletedAsync(TestContext.Current.CancellationToken);
        await _db.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var dto = new AdminCreateRequestDto()
        {
            Name = "Test Admin",
            Phone = "+45 12 34 57 57",
            Email = "admin@example.com",
            Password = "supersecret"
        };

        var created = await _adminService.CreateAsync(dto);

        var adminInDb = await _db.Admins
            .FirstAsync(a => a.Adminid == created.AdminId, TestContext.Current.CancellationToken);
        
        Assert.Equal(dto.Name, adminInDb.Name);
        Assert.Equal(dto.Phone, adminInDb.Phone);
        Assert.Equal(dto.Email.ToLowerInvariant(), adminInDb.Email);
        Assert.False(adminInDb.Isdeleted);
        Assert.Null(adminInDb.Deletedat);
    }

    [Fact]
    public async Task UpdateAsync_Updates_Name_Phone_Email()
    {
        await _db.Database.EnsureDeletedAsync(TestContext.Current.CancellationToken);
        await _db.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var createDto = new AdminCreateRequestDto()
        {
            Name = "Old Name",
            Phone = "111",
            Email = "old@example.com",
            Password = "secret123"
        };

        var created = await _adminService.CreateAsync(createDto);

        var updateDto = new AdminUpdateRequestDto()
        {
            Name = "New Name",
            Phone = "222",
            Email = "new@example.com"
        };

        var updated = await _adminService.UpdateAsync(created.AdminId, updateDto);

        var adminInDb = await _db.Admins
            .FirstAsync(a => a.Adminid == created.AdminId, TestContext.Current.CancellationToken);
        
        Assert.Equal("New Name", adminInDb.Name);
        Assert.Equal("222", adminInDb.Phone);
        Assert.Equal("new@example.com", adminInDb.Email);
        Assert.Equal(updated.AdminId, adminInDb.Adminid);
    }

    [Fact]
    public async Task SoftDeleteAsync_Marks_Admin_As_Deleted()
    {
        await _db.Database.EnsureDeletedAsync(TestContext.Current.CancellationToken);
        await _db.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var createDto = new AdminCreateRequestDto()
        {
            Name = "To Delete",
            Phone = "333",
            Email = "delete@example.com",
            Password = "secret123"
        };

        var created = await _adminService.CreateAsync(createDto);

        await _adminService.SoftDeleteAsync(created.AdminId);

        var adminInDb = await _db.Admins
            .FirstAsync(a => a.Adminid == created.AdminId, TestContext.Current.CancellationToken);
        
        Assert.True(adminInDb.Isdeleted);
        Assert.NotNull(adminInDb.Deletedat);
    }
}