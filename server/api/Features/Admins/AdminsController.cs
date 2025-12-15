using api.Features.Admins.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Admins;

    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")] // => api/admins
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        public async Task<ActionResult<AdminResponseDto>> Create([FromBody] AdminCreateRequestDto dto)
        {
            var created = await _adminService.CreateAsync(dto);
            return CreatedAtAction(nameof(Create), new { id = created.AdminId }, created);
            // later you can add GetById and use that here instead
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<AdminResponseDto>> Update(Guid id, [FromBody] AdminUpdateRequestDto dto)
        {
            var updated = await _adminService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _adminService.SoftDeleteAsync(id);
            return NoContent();
        }
    }
