using System.Linq;
using api.Features.Admins.Dtos;
using dataaccess.Entities;

namespace api.Features.Admins;

public static class AdminMapping
{
    public static AdminResponseDto ToAdminResponseDto(this Admin entity)
    {
        return new AdminResponseDto
        {
            AdminId = entity.Adminid,
            Name = entity.Name,
            Phone = entity.Phone,
            Email = entity.Email,
            CreatedAt = entity.Createdat,
            UpdatedAt = entity.Updatedat
        };
    }

    public static List<AdminResponseDto> ToAdminResponseDtos(this IEnumerable<Admin> entities)
    {
        return entities.Select(a => a.ToAdminResponseDto()).ToList();
    }
    
}