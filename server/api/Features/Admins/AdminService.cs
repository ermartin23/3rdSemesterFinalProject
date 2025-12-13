using api.Features.Admins.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Admins;

public class AdminService : IAdminService
{
    private readonly MyDbContext _db;

    public AdminService(MyDbContext db)
    {
        _db = db;
    }

    public async Task<AdminResponseDto> CreateAsync(AdminCreateRequestDto dto)
    {
        var now = DateTime.UtcNow;

        var admin = new Admin
        {
            Adminid = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Phone = dto.Phone.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            Password = dto.Password.Trim(), // Later HASH this!!!!
            Createdat = now,
            Updatedat = now,
            Isdeleted = false,
            Deletedat = null
        };

        _db.Admins.Add(admin);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("ADMIN DB ERROR: " + ex.InnerException?.Message);
            throw;
        }

        return admin.ToAdminResponseDto();
    }

    public async Task<AdminResponseDto> UpdateAsync(Guid id, AdminUpdateRequestDto dto)
    {
        var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Adminid == id && !a.Isdeleted);

        if (admin == null)
        {
            throw new KeyNotFoundException($"Admin with id {id} not found.");
        }

        admin.Name = dto.Name.Trim();
        admin.Phone = dto.Phone.Trim();
        admin.Email = dto.Email.Trim().ToLowerInvariant();
        admin.Updatedat = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return admin.ToAdminResponseDto();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Adminid == id && !a.Isdeleted);

        if (admin == null)
        {
            throw new KeyNotFoundException($"Admin with id {id} not found.");
        }

        var now = DateTime.UtcNow;

        admin.Isdeleted = true;
        admin.Deletedat = now;
        admin.Updatedat = now;

        await _db.SaveChangesAsync();
    }

}