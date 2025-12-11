using api.Features.Admins.Dtos;

namespace api.Features.Admins;

public interface IAdminService
{
    Task<AdminResponseDto> CreateAsync(AdminCreateRequestDto dto);
    Task<AdminResponseDto> UpdateAsync(Guid id, AdminUpdateRequestDto dto);
    Task SoftDeleteAsync(Guid id);
}